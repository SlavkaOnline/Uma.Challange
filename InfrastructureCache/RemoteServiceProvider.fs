namespace InfrastructureCache

open System
open System.Threading.Tasks
open System.Threading
open Newtonsoft.Json
open Infrastructure.Contracts

module private Helper = 
    let rec retry<'b>(task :unit -> Task<'b>) :Async<'b> =
        async {
             let! result = task().ContinueWith((fun (t :Task<'b>) -> 
                                                    async {
                                                         if t.IsCanceled then 
                                                            return! retry(task)
                                                         else 
                                                            return t.Result
                                                        } |> Async.StartAsTask )
                                                ) |> Async.AwaitTask
             return! result |> Async.AwaitTask
        }


type private CacheValue<'a> = {
    Value : 'a
    Expire :DateTime
}

type private CacheAgentCommand<'a> = 
            |  GetValue of path :string * AsyncReplyChannel<Task<CacheValue<'a>>>
            |  SetValue of value : CacheValue<'a>
            |  Clear

type private CacheAgentState<'a> =
            | Empty
            | HasValue of value: 'a
            | AwaitResponse of Task<'a>

type RemoteServiceProvider<'a>(client :IRemoteServiceClient<'a>) =   
    let client = client
   
    let cacheAgent = MailboxProcessor<CacheAgentCommand<'a>>.Start(fun inbox -> 
            
            let makeRemoteRequest (path :string) = 
                let tcs = new TaskCompletionSource<CacheValue<'a>>()
                                      
                Async.StartWithContinuations(
                    async {
                           return! client.Get(path) |> Async.AwaitTask
                    },
                    (fun result ->
                                  let value = {Value = result; Expire = DateTime.UtcNow.AddSeconds(30.0)}
                                  inbox.Post <| SetValue value
                                  tcs.SetResult value
                                  ),
                    (fun e -> tcs.SetException e),
                    (fun c -> ())
                    )
                
                tcs.Task
                
            let rec loop state = async {
                let! message = inbox.Receive() 
                match message with
                    | GetValue(path, channel) ->
                                  match state with
                                  | Empty ->
                                      let awaiter = makeRemoteRequest path                                
                                      channel.Reply awaiter
                                      return! loop <| AwaitResponse awaiter
                                  | HasValue value ->
                                      if value.Expire > DateTime.UtcNow then
                                          channel.Reply <| Task.FromResult value
                                          return! loop state
                                      else
                                          let awaiter = makeRemoteRequest path                                
                                          channel.Reply awaiter
                                          return! loop <| AwaitResponse awaiter
                                          
                                  | AwaitResponse t ->
                                      channel.Reply t
                                      return! loop state
                    | SetValue value -> 
                                  return! loop <| HasValue value
                    | Clear ->
                        return! loop <| Empty
                return! loop state
            }
            loop Empty
    )
    
    member this.Get(path :string) : Task<'a * DateTime> =   
        async {
            let! result = cacheAgent.PostAndTryAsyncReply ((fun replyChannel -> GetValue(path, replyChannel)), 30000)
            match result with
            | Some awaiter -> let! result = awaiter |> Async.AwaitTask
                              return (result.Value, result.Expire)

            | None -> 
                    return! Task.FromCanceled<'a * DateTime>(CancellationToken.None) |> Async.AwaitTask
        } |> Async.StartAsTask
    
    member this.GetWithRetry(path :string) : Task<'a * DateTime> =       
        Helper.retry(fun _ -> this.Get(path))
        |> Async.StartAsTask
        