using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

public class ApiManager
{
    private List<Article> articles = new List<Article>();

    public void HandleRequest(HttpListenerContext context)
    {
        string endpoint = context.Request.Url.LocalPath.ToLower();
        string method = context.Request.HttpMethod.ToUpper();

        switch (endpoint)
        {
            case "/articles":
                HandleArticlesEndpoint(context, method);
                break;
            default:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                break;
        }
    }

    private void HandleArticlesEndpoint(HttpListenerContext context, string method)
    {
        switch (method)
        {
            case "GET":
                GetArticles(context);
                break;
            case "POST":
                CreateArticle(context);
                break;
            case "PUT":
                UpdateArticle(context);
                break;
            case "DELETE":
                DeleteArticle(context);
                break;
            default:
                context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                break;
        }
    }

    private void GetArticles(HttpListenerContext context)
    {
        // Envoyer la liste des articles en tant que réponse JSON
        SendJsonResponse(context, articles);
    }

    private void CreateArticle(HttpListenerContext context)
    {
        // Lire le corps de la requête pour obtenir les données de l'article
        Article newArticle = ReadRequestBody<Article>(context);

        // Ajouter l'article à la liste
        articles.Add(newArticle);

        // Répondre avec l'article nouvellement créé
        SendJsonResponse(context, newArticle);
    }

    private void UpdateArticle(HttpListenerContext context)
    {
        // Lire le corps de la requête pour obtenir les données de l'article mis à jour
        Article updatedArticle = ReadRequestBody<Article>(context);

        // Extraire l'ID de l'article à mettre à jour à partir de l'URL
        int articleId = GetArticleIdFromUrl(context.Request.Url.LocalPath);

        // Mettre à jour l'article
        UpdateArticleById(articleId, updatedArticle);

        // Répondre avec l'article mis à jour
        SendJsonResponse(context, updatedArticle);
    }

    private void DeleteArticle(HttpListenerContext context)
    {
        // Extraire l'ID de l'article à supprimer à partir de l'URL
        int articleId = GetArticleIdFromUrl(context.Request.Url.LocalPath);

        // Supprimer l'article
        DeleteArticleById(articleId);

        // Répondre avec un message de succès
        SendTextResponse(context, "Article supprimé avec succès");
    }

    private void UpdateArticleById(int id, Article updatedArticle)
    {
        Article articleToUpdate = articles.Find(article => article.ID == id);

        if (articleToUpdate != null)
        {
            // Mettre à jour les propriétés de l'article
            articleToUpdate.Nom = updatedArticle.Nom;
            articleToUpdate.Quantite = updatedArticle.Quantite;
        }
    }

    private void DeleteArticleById(int id)
    {
        articles.RemoveAll(article => article.ID == id);
    }

    private int GetArticleIdFromUrl(string url)
    {
        // Extraire l'ID de l'URL, par exemple, "/articles/1" renverra 1
        string[] segments = url.Split('/');
        if (segments.Length >= 3 && int.TryParse(segments[2], out int articleId))
        {
            return articleId;
        }
        return -1;
    }

    private T ReadRequestBody<T>(HttpListenerContext context)
    {
        // Lire le corps de la requête et désérialiser le JSON en objet T
        using (var body = context.Request.InputStream)
        {
            using (var reader = new System.IO.StreamReader(body, context.Request.ContentEncoding))
            {
                string json = reader.ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
            }
        }
    }

    private void SendJsonResponse<T>(HttpListenerContext context, T responseData)
    {
        // Convertir l'objet en JSON et l'envoyer en tant que réponse
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(responseData);
        SendTextResponse(context, json, "application/json");
    }

    private void SendTextResponse(HttpListenerContext context, string responseText, string contentType = "text/plain")
    {
        // Convertir le texte en tableau d'octets et l'envoyer en tant que réponse
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseText);
        context.Response.ContentType = contentType;
        context.Response.ContentEncoding = System.Text.Encoding.UTF8;
        context.Response.ContentLength64 = buffer.Length;
        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        context.Response.OutputStream.Close();
    }
}


public class Article
{
    public int ID { get; set; }
    public string Nom { get; set; }
    public int Quantite { get; set; }
}
