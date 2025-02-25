﻿// hot-reload
[<AutoOpen>]
module Fun.Blazor.Docs.Wasm.App

open FSharp.Data.Adaptive
open Fun.Result
open Fun.Blazor
open Fun.Blazor.Router
open MudBlazor
open Fun.Blazor.Docs.Controls
open Fun.Blazor.Docs.Wasm


let private isReadyIndicator =
    html.comp (fun (hook: IComponentHook) ->
        let mutable showMessage = true

        hook.OnFirstAfterRender.Add(fun _ ->
            showMessage <- false
            hook.StateHasChanged()
        )

        div {
            if showMessage then
                div {
                    style { margin -40 10 40 10 }
                    MudProgressLinear'() {
                        Color Color.Warning
                        Indeterminate true
                    }
                    spaceV2
                    MudText'() {
                        Color Color.Warning
                        Typo Typo.subtitle2
                        ".NET WASM is still loading. You can interact in this page after it's fully loaded."
                    }
                    MudText'() {
                        Color Color.Info
                        Typo Typo.body2
                        "Current page is prerendered."
                    }
                }
        }
    )


let app =
    html.inject (fun (hook: IComponentHook, shareStore: IShareStore) ->
        hook.SetPrerenderStore(fun _ -> hook.SetDataToPrerenderStore(nameof hook.DocsTree, hook.DocsTree.Value))

        let isOpen = cval true

        let theme =
            adaptiview () {
                let! isDark = shareStore.IsDarkMode

                MudThemeProvider'() { Theme(if isDark then Theme.darkTheme else Theme.defaultTheme) }

                if isDark then
                    stylesheet "css/github-markdown-dark.css"
                else
                    stylesheet "css/github-markdown-light.css"
            }

        let menuBtn =
            adaptiview () {
                let! isOpen, setIsOpen = isOpen.WithSetter()
                MudIconButton'() {
                    Icon Icons.Material.Filled.Menu
                    Color Color.Inherit
                    Edge Edge.Start
                    OnClick(fun _ -> setIsOpen (not isOpen))
                }
            }

        let appBar =
            MudAppBar'() {
                style {
                    custom "backdrop-filter" "blur(15px)"
                    backgroundColor "transparent"
                    color Theme.primaryColor
                }
                Elevation 25
                Dense true
                childContent [
                    menuBtn
                    MudText'() {
                        Typo Typo.h6
                        Color Color.Default
                        "Fun Blazor"
                    }
                    MudImage'() {
                        style { margin 10 }
                        Height 35
                        Width 35
                        Src $"fun-blazor.png"
                    }
                    MudSpacer'.create ()
                    adaptiview () {
                        let! isDark, setIsDark = shareStore.IsDarkMode.WithSetter()
                        MudIconButton'() {
                            Color Color.Inherit
                            Icon(if isDark then Icons.Filled.Brightness4 else Icons.Filled.Brightness3)
                            OnClick(fun _ -> setIsDark (not isDark))
                        }
                    }
                    MudIconButton'() {
                        Icon Icons.Custom.Brands.GitHub
                        Color Color.Inherit
                        Link "https://github.com/slaveOftime/Fun.Blazor"
                    }
                ]
            }

        let drawer =
            adaptiview () {
                let! binding = isOpen.WithSetter()
                MudDrawer'() {
                    Open' binding
                    Elevation 25
                    PreserveOpenState true
                    ClipMode DrawerClipMode.Always
                    navmenu
                }
            }

        let routesView =
            adaptiview () {
                match! hook.GetOrLoadDocsTree() with
                | LoadingState.NotStartYet -> notFound
                | LoadingState.Loading -> MudProgressLinear'.create ()
                | LoadingState.Loaded docs
                | LoadingState.Reloading docs ->
                    let routes = [ docRouting docs; Demos.GiraffeStyleRouter.demoRouting ]
                    html.route (
                        docs,
                        [
                            // For host on slaveoftime.fun server mode
                            yield! routes
                            // For host on github-pages WASM mode
                            subRouteCi "/Fun.Blazor.Docs" routes
                            routeAny (
                                docs
                                |> Seq.tryHead
                                |> Option.map (
                                    function
                                    | DocTreeNode.Category (doc, _, _)
                                    | DocTreeNode.Doc doc -> docView doc
                                )
                                |> Option.defaultValue notFound
                            )
                        ]
                    )
            }


        div.create [
            theme
            MudDialogProvider'.create ()
            MudSnackbarProvider'.create ()
            MudLayout'.create [
                appBar
                drawer
                MudMainContent'() {
                    style {
                        paddingTop 100
                        paddingBottom 64
                    }
                    isReadyIndicator
                    errorBundary routesView
                    MudScrollToTop'() {
                        TopOffset 400
                        MudFab'() {
                            Icon Icons.Material.Filled.KeyboardArrowUp
                            Color Color.Primary
                        }
                    }
                }
            ]
            interopScript
            styleElt { ruleset ".markdown-body li" { listStyleTypeInitial } }
        ]
    )
