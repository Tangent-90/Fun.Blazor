namespace Fun.Blazor.Docs.Server

open Bolero.Server.Html
open Fun.Blazor


type Index () =
    inherit Bolero.Component()

    override this.Render() = Docs.Wasm.App.app.Node().ToBolero()


    static member page =
        doctypeHtml [] [
            html.html ("en", [
                html.head [
                    html.title "Fun Blazor"
                    html.baseUrl "/"
                    html.meta [ attr.charsetUtf8 ]
                    html.meta [ attr.name "viewport"; attr.content "width=device-width, initial-scale=1.0" ]
                ]
                html.body [
                    attr.styles [ style.margin 0 ]
                    attr.childContent [
                        html.bolero rootComp<Index>
                        html.bolero Bolero.Server.Html.boleroScript
                    ]
                    
                    html.stylesheet "css/google-font.css"
                    html.stylesheet "_content/MudBlazor/MudBlazor.min.css"
                    html.script "_content/MudBlazor/MudBlazor.min.js"

                    html.stylesheet "css/github-markdown.css"
                    html.stylesheet "css/prism-night-owl.css"
                    html.script "https://cdnjs.cloudflare.com/ajax/libs/prism/1.23.0/components/prism-core.min.js"
                    html.script "https://cdnjs.cloudflare.com/ajax/libs/prism/1.23.0/plugins/autoloader/prism-autoloader.min.js"
                ]
            ])
            |> html.toBolero
        ]
