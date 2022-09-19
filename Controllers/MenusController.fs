namespace LunchApi.Controllers

open System.Net.Http
open System.Text
open HtmlAgilityPack
open Microsoft.AspNetCore.Mvc

type MenuType =
    | Main = 0
    | Soup = 1

type RestaurantSetting = { Name: string; Url: string }

type Menu =
    { Price: string
      Description: string
      Type: MenuType }

type Restaurant = { Id: int; Menu: Menu [] }

[<ApiController>]
[<Route("[controller]")>]
type MenusController() =
    inherit ControllerBase()

    [<HttpPost>]
    member _.Get(ids: int []) =
        Encoding.RegisterProvider CodePagesEncodingProvider.Instance
        let httpClient = new HttpClient()

        let downloadContent (url: string) =
            async {
                let! response = httpClient.GetAsync url |> Async.AwaitTask

                let! content =
                    response.Content.ReadAsStringAsync()
                    |> Async.AwaitTask

                return content
            }
            |> Async.RunSynchronously

        let parseHtml (node: HtmlNode) nodeType (menuType: MenuType) =
            node.SelectNodes ".//tr"
            |> Seq.filter (fun node ->
                node.GetClasses()
                |> (fun classes -> classes |> Seq.contains nodeType))
            |> Seq.map (fun node ->
                { Type = menuType
                  Description =
                    node.SelectNodes ".//td"
                    |> Seq.find (fun node -> node.GetClasses() |> Seq.contains "food")
                    |> (fun node -> node.InnerText)

                  Price =
                      node.SelectNodes ".//td"
                      |> Seq.find (fun node -> node.GetClasses() |> Seq.contains "prize")
                      |> (fun node -> node.InnerText) })

        let loadHtml (content: string) =
            let document = HtmlDocument()
            document.LoadHtml content
            document.DocumentNode

        let urls =
            ids
            |> Seq.map (fun id ->
                (id,
                 "http://www.menicka.cz/api/iframe/?id="
                 + id.ToString()
                 + "&bg=vhite&color=black&size=18&datum=dnes"))

        let res =
            urls
            |> Seq.map (fun (id, url) -> (id, downloadContent url))
            |> Seq.map (fun (id, content) -> (id, loadHtml content))
            |> Seq.map (fun (id, html) ->
                { Id = id
                  Menu =
                    [ (parseHtml html "soup" MenuType.Soup)
                      (parseHtml html "main" MenuType.Main) ]
                    |> Seq.concat
                    |> Seq.toArray })

        res
