﻿[<AutoOpen>]
module Fun.Blazor.Docs.Wasm.Components.SourceSection

open System.Net.Http
open System.Threading.Tasks
open FSharp.Control.Reactive
open Microsoft.Extensions.Configuration
open Microsoft.AspNetCore.Hosting
open MudBlazor
open Fun.Blazor
open Fun.Result


let private deferredObserve (data: Task<Result<string, string>>) =
    data
    |> Async.AwaitTask
    |> Async.Catch
    |> Observable.ofAsync
    |> Observable.map (
        function
        | Choice1Of2 (Ok x) -> DeferredState.Loaded x
        | Choice1Of2 (Error x) -> DeferredState.LoadFailed x
        | Choice2Of2 x -> DeferredState.LoadFailed x.Message
    )


let private version = "2.0.0"


let private getFromHostServer (env: IHostingEnvironment) (fileName: string) =
    let path =
#if DEBUG
        $"{env.ContentRootPath}/../Fun.Blazor.Docs.Wasm/wwwroot/code-docs/{fileName}.html"
#else
        $"{env.ContentRootPath}/wwwroot/code-docs/{fileName}.html"
#endif

    System.IO.File.ReadAllTextAsync(path) |> Task.map Ok |> deferredObserve


let private getFromGithub (fileName: string) =
    let client = new HttpClient()

    let host =
#if DEBUG
        "https://localhost:5001"
#else
        "https://slaveoftime.github.io/Fun.Blazor.Docs"
#endif

    client.GetAsync($"{host}/code-docs/{fileName}.html?v={version}")
    |> Task.bind (fun x ->
        if x.IsSuccessStatusCode then
            x.Content.ReadAsStringAsync() |> Task.map Ok
        else
            x.StatusCode |> string |> Error |> Task.retn
    )
    |> deferredObserve


let sourceSection fileName =
    html.inject (
        fileName,
        fun (env: IHostingEnvironment, config: IConfiguration, globalStore: IGlobalStore) ->
            let code =
                globalStore.CreateDeferred(
                    $"code-source-{fileName}",
                    fun () ->
                        if config <> null && config.GetValue<bool>("BlazorServerHost", false) then
                            getFromHostServer env fileName
                        else
                            getFromGithub fileName
                )

            html.watch (
                code,
                function
                | DeferredState.Loading ->
                    MudProgressLinear'() {
                        Color Color.Primary
                        Indeterminate true
                    }
                | DeferredState.Loaded codes ->
                    div {
                        article {
                            class' "markdown-body"
                            html.raw codes
                        }
                        script { src "https://cdnjs.cloudflare.com/ajax/libs/prism/1.23.0/components/prism-core.min.js" }
                        script { src "https://cdnjs.cloudflare.com/ajax/libs/prism/1.23.0/plugins/autoloader/prism-autoloader.min.js" }
                    }
                | DeferredState.LoadFailed e ->
                    MudAlert'() {
                        Severity Severity.Error
                        childContent e
                    }
                | _ -> html.none
            )
    )
