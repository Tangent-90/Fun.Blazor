﻿namespace Fun.Blazor

open System
open System.Threading.Tasks
open Fun.Blazor
open HtmlTemplate.Internals


[<AutoOpen>]
module Utils =
    let callback (fn: 'T -> unit) : ArgMkAttrWithName = fun name -> html.callback<'T> (name, fn)

    let callbackTask (fn: 'T -> Task) : ArgMkAttrWithName = fun name -> html.callbackTask<'T> (name, fn)


type Template =
    /// This will generate bolero node tree and put them in caches
    /// For none standard event you can prefix with 'on' to indicate it is a callback. onsl-change="{callback (fun (e: SlChangeEventArgs) -> ...)}".
    /// For none standard event, you also need to wireup the event argument type by: https://docs.microsoft.com/en-us/aspnet/core/blazor/components/event-handling?view=aspnetcore-6.0#custom-event-arguments
    /// But it is only supported in aspnet 6. So currently you cannot get the event args very easily.
    static member html(html: FormattableString) =
        let args = html.GetArguments()
        if args.Length = 0 then
            NodeRenderFragment(fun _ builder index ->
                builder.AddMarkupContent(index, html.Format)
                index + 1
            )
        else
            let nodes =
                caches.GetOrAdd(html.Format.GetHashCode(), Func<int, NodeItem list>(fun _ -> parseNodes html.Format))
            rebuildNodes nodes args
