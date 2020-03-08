# C# Client Library

Version: ![Nuget](https://img.shields.io/nuget/v/Squidex.ClientLibrary?style=flat-square)

Also available on [nuget.org](https://www.nuget.org/packages/Squidex.ClientLibrary/).

## How to use it?

Our recommendation is to have a look to the other samples to see how it works, but here is a short introduction.

The client library uses the following namespace: `Squidex.ClientLibrary`

### Initial Setup
Prerequisite: Have an existing app in https://cloud.squidex.io (or in your local Squidex setup) with schemas and content.

The easiest way to start with Squidex is to automatically generate C# classes based on your schemas in Squidex. It is possible to define C# classes manually but you will need to sync them after each schema update. 

1. Clone the squidex-samples repository from https://github.com/Squidex/squidex-samples
2. Open `squidex-samples/csharp/Squidex.ClientLibrary/Squidex.ClientLibrary.sln`
3. Set `CodeGeneration` as your startup project
4. Open `Program.cs`, insert you app name, adjust the outputPath to point into the project where you want to use Squidex, and start the `CodeGeneration` project.
5. Go to https://cloud.squidex.io, open your app and browse to *settings -> clients* to see the credentials necessary to enable the Squidex.ClientLibrary to retrieve content from Squidex.

### Usage
1. Create a `SquidexClientManager` using the credentials from the step above. 

````
var clientManager =  
    new SquidexClientManager(  
        new SquidexOptions  
        {  
            AppName = "...",  
            ClientId = "...",  
            ClientSecret = "...",  
            Url = "https://cloud.squidex.io"  
        });  
````

Let's assume you have a schema called *Blog*. You can retrieve content like this:

````
var httpClient = clientManager.CreateHttpClient(); // try to reuse this instance for all clients due to performance reasons, see https://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/  
var yourBlogClient = new BlogClient(httpClient); // name of the class depends on your schema's name and, therefore, the generated classes  
var yourData = yourBlogClient.QueryBlogContentsAsync(); // replace `Blog` with the name of your schema  
````

### Further Usage

#### Manually Create Schema Classes

For each schema you have to create two classes.

1. The data object:

````
public sealed class BlogPostData
{
    // Squidex delivers hash maps for each field, but the 
    // InvariantConverter helps to simplify the model.
    [JsonConverter(typeof(InvariantConverter))]
    public string Title { get; set; }

    [JsonConverter(typeof(InvariantConverter))]
    public string Slug { get; set; }

    // For localizable fields you can use dictionaries.
    public Dictionary<string, string> Text { get; set; }
}
````

2. The entity object

````
public sealed class BlogPost : Content<BlogPostData>
{
}
````


### 2. Use Squidex.ClientLibrary with manually specifying the model classes



````
var client = clientManager.CreateContentsClient<BlogPost, BlogPostData>("posts");

// Read posts pages
var posts = await client.GetAsync(page * pageSize, pageSize);

// Read post by id.
var post = client.GetAsync(id);

// Read posts by slug.
var posts = await client.GetAsync(filter: $"data/slug/iv eq '{slug}'");
````
