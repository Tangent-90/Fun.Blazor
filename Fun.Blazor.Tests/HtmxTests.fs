﻿module Fun.Blazor.Tests.HtmxTests

open Xunit
open Fun.Htmx
open Fun.Blazor
open Bunit


[<Fact>]
let ``TriggerBuilder should work`` () =
    let context = new TestContext()

    let result =
        context.RenderNode(
            fragment {
                div {
                    hxGet "/test"
                    hxTrigger (
                        HxTrigger(
                            hxEvt.load,
                            sse = true,
                            filter = "ctlKey",
                            once = true,
                            changed = true,
                            delayMs = 100,
                            throttleMs = 200,
                            from = "#from",
                            target = "#target",
                            consume = true,
                            queue = HxQueue.First,
                            root = "#root",
                            threshold = 0.2
                        )
                            .AddTrigger(hxEvt.mouse.click)
                    )
                }
                div {
                    hxTrigger'(hxEvt.mouse.dblclick, once = true)
                        .AddTrigger(hxEvt.mouse.click)
                        .AddTrigger(hxEvt.mouse.mousemove)
                        .AddTrigger(hxEvt.mouse.mouseover)
                    "TEST"
                }
                div {
                    hxTrigger hxEvt.revealed
                    hxSwap_beforebegin'().Swap(1).Settle(2).FocusScroll(true).ScrollTop("#target").ShowTop()
                }
                div {
                    hxPost "/test2"
                    hxTarget_find "#find"
                    hxSwap_afterend'(OOB).FocusScroll(true)
                }
                div {
                    hxSwap_delete
                }
                div {
                    hxSwap_delete OOB
                }
            }
        )

    result.MarkupMatches(
        """
        <div hx-get="/test" hx-trigger="sse:load[ctlKey] once changed delay:100ms throttle:200ms from:#from target:#target consume queue:first, click"></div>
        <div hx-trigger="dblclick once, click, mousemove, mouseover">TEST</div>
        <div hx-trigger="revealed" hx-swap="beforebegin swap:1 settle:2 focus-scroll:true scroll:#target:top show:top"></div>
        <div hx-post="/test2" hx-target="find #find" hx-swap-oob="afterend focus-scroll:true"></div>
        <div hx-swap="delete"></div>
        <div hx-swap-oob="delete"></div>
        """
    )
