# C# Client Library

Version: 1.5.0

Also available on [nuget.org](https://www.nuget.org/packages/Squidex.ClientLibrary/).

## How to use it?

Our recommendation is to have a look to the other samples to see how it works, but here is a short introduction.

The client library uses the following namespace: `Squidex.ClientLibrary`

### 1. Create your model.

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
public sealed class BlogPost : SquidexEntityBase<BlogPostData>
{
}
````


### 2. Initialize the client manager

````
var clientManager =
    new SquidexClientManager(
        options.Url,            // e.g. https://cloud.squidex.io
        options.AppName,        // The name of your app.
        options.ClientId,       // The name of your client.
        options.ClientSecret);  // The secret of your client.
````

### 3. Create a client for your schema

````
var client = clientManager.GetClient<BlogPost, BlogPostData>("posts");
````

### 4. Use the client

````
// Read posts pages
var posts = await client.GetAsync(page * pageSize, pageSize);

// Read post by id.
var post = client.GetAsync(id);

// Read posts by slug.
var posts = await client.GetAsync(filter: $"data/slug/iv eq '{slug}'");
````