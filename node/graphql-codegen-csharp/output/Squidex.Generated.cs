using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Squidex {
  public class GraphQL {
    
    #region ApplicationMutations
    /// <summary>
    /// The app mutations.
    /// </summary>
    public class ApplicationMutations {
      #region members
      /// <summary>
      /// Change a pages content.
      /// </summary>
      [JsonProperty("changePagesContent")]
      public Pages changePagesContent { get; set; }
    
      /// <summary>
      /// Change a posts content.
      /// </summary>
      [JsonProperty("changePostsContent")]
      public Posts changePostsContent { get; set; }
    
      /// <summary>
      /// Creates an pages content.
      /// </summary>
      [JsonProperty("createPagesContent")]
      public Pages createPagesContent { get; set; }
    
      /// <summary>
      /// Creates an posts content.
      /// </summary>
      [JsonProperty("createPostsContent")]
      public Posts createPostsContent { get; set; }
    
      /// <summary>
      /// Delete an pages content.
      /// </summary>
      [JsonProperty("deletePagesContent")]
      public EntitySavedResultDto deletePagesContent { get; set; }
    
      /// <summary>
      /// Delete an posts content.
      /// </summary>
      [JsonProperty("deletePostsContent")]
      public EntitySavedResultDto deletePostsContent { get; set; }
    
      /// <summary>
      /// Patch an pages content by id.
      /// </summary>
      [JsonProperty("patchPagesContent")]
      public Pages patchPagesContent { get; set; }
    
      /// <summary>
      /// Patch an posts content by id.
      /// </summary>
      [JsonProperty("patchPostsContent")]
      public Posts patchPostsContent { get; set; }
    
      /// <summary>
      /// Publish a pages content.
      /// </summary>
      [Obsolete("Use 'changePagesContent' instead")]
      [JsonProperty("publishPagesContent")]
      public Pages publishPagesContent { get; set; }
    
      /// <summary>
      /// Publish a posts content.
      /// </summary>
      [Obsolete("Use 'changePostsContent' instead")]
      [JsonProperty("publishPostsContent")]
      public Posts publishPostsContent { get; set; }
    
      /// <summary>
      /// Update an pages content by id.
      /// </summary>
      [JsonProperty("updatePagesContent")]
      public Pages updatePagesContent { get; set; }
    
      /// <summary>
      /// Update an posts content by id.
      /// </summary>
      [JsonProperty("updatePostsContent")]
      public Posts updatePostsContent { get; set; }
    
      /// <summary>
      /// Upsert an pages content by id.
      /// </summary>
      [JsonProperty("upsertPagesContent")]
      public Pages upsertPagesContent { get; set; }
    
      /// <summary>
      /// Upsert an posts content by id.
      /// </summary>
      [JsonProperty("upsertPostsContent")]
      public Posts upsertPostsContent { get; set; }
      #endregion
    }
    #endregion
    
    #region ApplicationQueries
    /// <summary>
    /// The app queries.
    /// </summary>
    public class ApplicationQueries {
      #region members
      /// <summary>
      /// Find an asset by id.
      /// </summary>
      [JsonProperty("findAsset")]
      public Asset findAsset { get; set; }
    
      /// <summary>
      /// Find an pages content by id.
      /// </summary>
      [JsonProperty("findPagesContent")]
      public Pages findPagesContent { get; set; }
    
      /// <summary>
      /// Find an posts content by id.
      /// </summary>
      [JsonProperty("findPostsContent")]
      public Posts findPostsContent { get; set; }
    
      /// <summary>
      /// Get assets.
      /// </summary>
      [JsonProperty("queryAssets")]
      public List<Asset> queryAssets { get; set; }
    
      /// <summary>
      /// Get assets and total count.
      /// </summary>
      [JsonProperty("queryAssetsWithTotal")]
      public AssetResultDto queryAssetsWithTotal { get; set; }
    
      /// <summary>
      /// Query content items by IDs across schemeas.
      /// </summary>
      [JsonProperty("queryContentsByIds")]
      public List<AllContents> queryContentsByIds { get; set; }
    
      /// <summary>
      /// Query pages content items.
      /// </summary>
      [JsonProperty("queryPagesContents")]
      public List<Pages> queryPagesContents { get; set; }
    
      /// <summary>
      /// Query pages content items with total count.
      /// </summary>
      [JsonProperty("queryPagesContentsWithTotal")]
      public PagesResultDto queryPagesContentsWithTotal { get; set; }
    
      /// <summary>
      /// Query posts content items.
      /// </summary>
      [JsonProperty("queryPostsContents")]
      public List<Posts> queryPostsContents { get; set; }
    
      /// <summary>
      /// Query posts content items with total count.
      /// </summary>
      [JsonProperty("queryPostsContentsWithTotal")]
      public PostsResultDto queryPostsContentsWithTotal { get; set; }
      #endregion
    }
    #endregion
    
    #region ApplicationSubscriptions
    public class ApplicationSubscriptions {
      #region members
      /// <summary>
      /// Subscribe to asset events.
      /// </summary>
      [JsonProperty("assetChanges")]
      public EnrichedAssetEvent assetChanges { get; set; }
    
      /// <summary>
      /// Subscribe to content events.
      /// </summary>
      [JsonProperty("contentChanges")]
      public EnrichedContentEvent contentChanges { get; set; }
      #endregion
    }
    #endregion
    
    #region Asset
    /// <summary>
    /// An asset
    /// </summary>
    public class Asset {
      #region members
      /// <summary>
      /// The timestamp when the object was created.
      /// </summary>
      [JsonProperty("created")]
      public DateTime created { get; set; }
    
      /// <summary>
      /// The user who created the object.
      /// </summary>
      [JsonProperty("createdBy")]
      public string createdBy { get; set; }
    
      /// <summary>
      /// The user who created the object.
      /// </summary>
      [JsonProperty("createdByUser")]
      public User createdByUser { get; set; }
    
      /// <summary>
      /// The edit token.
      /// </summary>
      [JsonProperty("editToken")]
      public string editToken { get; set; }
    
      /// <summary>
      /// The hash of the file. Can be null for old files.
      /// </summary>
      [JsonProperty("fileHash")]
      public string fileHash { get; set; }
    
      /// <summary>
      /// The file name of the asset.
      /// </summary>
      [JsonProperty("fileName")]
      public string fileName { get; set; }
    
      /// <summary>
      /// The size of the file in bytes.
      /// </summary>
      [JsonProperty("fileSize")]
      public int fileSize { get; set; }
    
      /// <summary>
      /// The file type (file extension) of the asset.
      /// </summary>
      [JsonProperty("fileType")]
      public string fileType { get; set; }
    
      /// <summary>
      /// The version of the file.
      /// </summary>
      [JsonProperty("fileVersion")]
      public int fileVersion { get; set; }
    
      /// <summary>
      /// The ID of the object (usually GUID).
      /// </summary>
      [JsonProperty("id")]
      public string id { get; set; }
    
      /// <summary>
      /// Determines if the uploaded file is an image.
      /// </summary>
      [Obsolete("Use 'type' field instead.")]
      [JsonProperty("isImage")]
      public bool isImage { get; set; }
    
      /// <summary>
      /// True, when the asset is not public.
      /// </summary>
      [JsonProperty("isProtected")]
      public bool isProtected { get; set; }
    
      /// <summary>
      /// The timestamp when the object was updated the last time.
      /// </summary>
      [JsonProperty("lastModified")]
      public DateTime lastModified { get; set; }
    
      /// <summary>
      /// The user who updated the object the last time.
      /// </summary>
      [JsonProperty("lastModifiedBy")]
      public string lastModifiedBy { get; set; }
    
      /// <summary>
      /// The user who updated the object the last time.
      /// </summary>
      [JsonProperty("lastModifiedByUser")]
      public User lastModifiedByUser { get; set; }
    
      /// <summary>
      /// The asset metadata.
      /// </summary>
      [JsonProperty("metadata")]
      public object metadata { get; set; }
    
      /// <summary>
      /// The type of the image.
      /// </summary>
      [JsonProperty("metadataText")]
      public string metadataText { get; set; }
    
      /// <summary>
      /// The mime type.
      /// </summary>
      [JsonProperty("mimeType")]
      public string mimeType { get; set; }
    
      /// <summary>
      /// The ID of the parent folder. Empty for files without parent.
      /// </summary>
      [JsonProperty("parentId")]
      public string parentId { get; set; }
    
      /// <summary>
      /// The height of the image in pixels if the asset is an image.
      /// </summary>
      [Obsolete("Use 'metadata' field instead.")]
      [JsonProperty("pixelHeight")]
      public int? pixelHeight { get; set; }
    
      /// <summary>
      /// The width of the image in pixels if the asset is an image.
      /// </summary>
      [Obsolete("Use 'metadata' field instead.")]
      [JsonProperty("pixelWidth")]
      public int? pixelWidth { get; set; }
    
      /// <summary>
      /// The file name as slug.
      /// </summary>
      [JsonProperty("slug")]
      public string slug { get; set; }
    
      /// <summary>
      /// The source URL of the asset.
      /// </summary>
      [JsonProperty("sourceUrl")]
      public string sourceUrl { get; set; }
    
      /// <summary>
      /// The asset tags.
      /// </summary>
      [JsonProperty("tags")]
      public List<string> tags { get; set; }
    
      /// <summary>
      /// The thumbnail URL to the asset.
      /// </summary>
      [JsonProperty("thumbnailUrl")]
      public string thumbnailUrl { get; set; }
    
      /// <summary>
      /// The type of the asset.
      /// </summary>
      [JsonProperty("type")]
      public AssetType type { get; set; }
    
      /// <summary>
      /// The URL to the asset.
      /// </summary>
      [JsonProperty("url")]
      public string url { get; set; }
    
      /// <summary>
      /// The version of the objec.
      /// </summary>
      [JsonProperty("version")]
      public int version { get; set; }
      #endregion
    }
    #endregion
    
    #region AssetResultDto
    /// <summary>
    /// List of assets and total count of assets.
    /// </summary>
    public class AssetResultDto {
      #region members
      /// <summary>
      /// The assets.
      /// </summary>
      [JsonProperty("items")]
      public List<Asset> items { get; set; }
    
      /// <summary>
      /// The total count of assets.
      /// </summary>
      [JsonProperty("total")]
      public int total { get; set; }
      #endregion
    }
    #endregion
    public enum AssetType {
      AUDIO,
      IMAGE,
      UNKNOWN,
      VIDEO
    }
    
    
    /// <summary>
    /// The structure of all content types.
    /// </summary>
    public interface Component {
      /// <summary>
      /// schemaId
      /// </summary>
      [JsonProperty("schemaId")]
      string schemaId { get; set; }
    
      /// <summary>
      /// schemaName
      /// </summary>
      [JsonProperty("schemaName")]
      string schemaName { get; set; }
    }
    
    /// <summary>
    /// The structure of all content types.
    /// </summary>
    public interface Content {
      /// <summary>
      /// created
      /// </summary>
      [JsonProperty("created")]
      DateTime created { get; set; }
    
      /// <summary>
      /// createdBy
      /// </summary>
      [JsonProperty("createdBy")]
      string createdBy { get; set; }
    
      /// <summary>
      /// data__dynamic
      /// </summary>
      [JsonProperty("data__dynamic")]
      object dataDynamic { get; set; }
    
      /// <summary>
      /// editToken
      /// </summary>
      [JsonProperty("editToken")]
      string editToken { get; set; }
    
      /// <summary>
      /// id
      /// </summary>
      [JsonProperty("id")]
      string id { get; set; }
    
      /// <summary>
      /// lastModified
      /// </summary>
      [JsonProperty("lastModified")]
      DateTime lastModified { get; set; }
    
      /// <summary>
      /// lastModifiedBy
      /// </summary>
      [JsonProperty("lastModifiedBy")]
      string lastModifiedBy { get; set; }
    
      /// <summary>
      /// newStatus
      /// </summary>
      [JsonProperty("newStatus")]
      string newStatus { get; set; }
    
      /// <summary>
      /// newStatusColor
      /// </summary>
      [JsonProperty("newStatusColor")]
      string newStatusColor { get; set; }
    
      /// <summary>
      /// status
      /// </summary>
      [JsonProperty("status")]
      string status { get; set; }
    
      /// <summary>
      /// statusColor
      /// </summary>
      [JsonProperty("statusColor")]
      string statusColor { get; set; }
    
      /// <summary>
      /// url
      /// </summary>
      [JsonProperty("url")]
      string url { get; set; }
    
      /// <summary>
      /// version
      /// </summary>
      [JsonProperty("version")]
      int version { get; set; }
    }
    
    #region EnrichedAssetEvent
    /// <summary>
    /// An asset event
    /// </summary>
    public class EnrichedAssetEvent {
      #region members
      /// <summary>
      /// The type of the asset.
      /// </summary>
      [JsonProperty("assetType")]
      public AssetType assetType { get; set; }
    
      /// <summary>
      /// The timestamp when the object was created.
      /// </summary>
      [JsonProperty("created")]
      public DateTime created { get; set; }
    
      /// <summary>
      /// The user who created the object.
      /// </summary>
      [JsonProperty("createdBy")]
      public string createdBy { get; set; }
    
      /// <summary>
      /// The user who created the object.
      /// </summary>
      [JsonProperty("createdByUser")]
      public User createdByUser { get; set; }
    
      /// <summary>
      /// The hash of the file. Can be null for old files.
      /// </summary>
      [JsonProperty("fileHash")]
      public string fileHash { get; set; }
    
      /// <summary>
      /// The file name of the asset.
      /// </summary>
      [JsonProperty("fileName")]
      public string fileName { get; set; }
    
      /// <summary>
      /// The size of the file in bytes.
      /// </summary>
      [JsonProperty("fileSize")]
      public int fileSize { get; set; }
    
      /// <summary>
      /// The file type (file extension) of the asset.
      /// </summary>
      [JsonProperty("fileType")]
      public string fileType { get; set; }
    
      /// <summary>
      /// The version of the file.
      /// </summary>
      [JsonProperty("fileVersion")]
      public int fileVersion { get; set; }
    
      /// <summary>
      /// The ID of the object (usually GUID).
      /// </summary>
      [JsonProperty("id")]
      public string id { get; set; }
    
      /// <summary>
      /// Determines if the uploaded file is an image.
      /// </summary>
      [Obsolete("Use 'type' field instead.")]
      [JsonProperty("isImage")]
      public bool isImage { get; set; }
    
      /// <summary>
      /// True, when the asset is not public.
      /// </summary>
      [JsonProperty("isProtected")]
      public bool isProtected { get; set; }
    
      /// <summary>
      /// The timestamp when the object was updated the last time.
      /// </summary>
      [JsonProperty("lastModified")]
      public DateTime lastModified { get; set; }
    
      /// <summary>
      /// The user who updated the object the last time.
      /// </summary>
      [JsonProperty("lastModifiedBy")]
      public string lastModifiedBy { get; set; }
    
      /// <summary>
      /// The user who updated the object the last time.
      /// </summary>
      [JsonProperty("lastModifiedByUser")]
      public User lastModifiedByUser { get; set; }
    
      /// <summary>
      /// The asset metadata.
      /// </summary>
      [JsonProperty("metadata")]
      public object metadata { get; set; }
    
      /// <summary>
      /// The mime type.
      /// </summary>
      [JsonProperty("mimeType")]
      public string mimeType { get; set; }
    
      /// <summary>
      /// The height of the image in pixels if the asset is an image.
      /// </summary>
      [Obsolete("Use 'metadata' field instead.")]
      [JsonProperty("pixelHeight")]
      public int? pixelHeight { get; set; }
    
      /// <summary>
      /// The width of the image in pixels if the asset is an image.
      /// </summary>
      [Obsolete("Use 'metadata' field instead.")]
      [JsonProperty("pixelWidth")]
      public int? pixelWidth { get; set; }
    
      /// <summary>
      /// The file name as slug.
      /// </summary>
      [JsonProperty("slug")]
      public string slug { get; set; }
    
      /// <summary>
      /// The source URL of the asset.
      /// </summary>
      [JsonProperty("sourceUrl")]
      public string sourceUrl { get; set; }
    
      /// <summary>
      /// The thumbnail URL to the asset.
      /// </summary>
      [JsonProperty("thumbnailUrl")]
      public string thumbnailUrl { get; set; }
    
      /// <summary>
      /// The type of the event.
      /// </summary>
      [JsonProperty("type")]
      public EnrichedAssetEventType? type { get; set; }
    
      /// <summary>
      /// The URL to the asset.
      /// </summary>
      [JsonProperty("url")]
      public string url { get; set; }
    
      /// <summary>
      /// The version of the objec.
      /// </summary>
      [JsonProperty("version")]
      public int version { get; set; }
      #endregion
    }
    #endregion
    public enum EnrichedAssetEventType {
      ANNOTATED,
      CREATED,
      DELETED,
      UPDATED
    }
    
    
    #region EnrichedContentEvent
    /// <summary>
    /// An content event
    /// </summary>
    public class EnrichedContentEvent {
      #region members
      /// <summary>
      /// The timestamp when the object was created.
      /// </summary>
      [JsonProperty("created")]
      public DateTime created { get; set; }
    
      /// <summary>
      /// The user who created the object.
      /// </summary>
      [JsonProperty("createdBy")]
      public string createdBy { get; set; }
    
      /// <summary>
      /// The user who created the object.
      /// </summary>
      [JsonProperty("createdByUser")]
      public User createdByUser { get; set; }
    
      /// <summary>
      /// The data of the content.
      /// </summary>
      [JsonProperty("data")]
      public object data { get; set; }
    
      /// <summary>
      /// The previous data of the content.
      /// </summary>
      [JsonProperty("dataOld")]
      public object dataOld { get; set; }
    
      /// <summary>
      /// The ID of the object (usually GUID).
      /// </summary>
      [JsonProperty("id")]
      public string id { get; set; }
    
      /// <summary>
      /// The timestamp when the object was updated the last time.
      /// </summary>
      [JsonProperty("lastModified")]
      public DateTime lastModified { get; set; }
    
      /// <summary>
      /// The user who updated the object the last time.
      /// </summary>
      [JsonProperty("lastModifiedBy")]
      public string lastModifiedBy { get; set; }
    
      /// <summary>
      /// The user who updated the object the last time.
      /// </summary>
      [JsonProperty("lastModifiedByUser")]
      public User lastModifiedByUser { get; set; }
    
      /// <summary>
      /// The new status of the content.
      /// </summary>
      [JsonProperty("newStatus")]
      public string newStatus { get; set; }
    
      /// <summary>
      /// The status of the content.
      /// </summary>
      [JsonProperty("status")]
      public string status { get; set; }
    
      /// <summary>
      /// The type of the event.
      /// </summary>
      [JsonProperty("type")]
      public EnrichedContentEventType? type { get; set; }
    
      /// <summary>
      /// The version of the objec.
      /// </summary>
      [JsonProperty("version")]
      public int version { get; set; }
      #endregion
    }
    #endregion
    public enum EnrichedContentEventType {
      CREATED,
      DELETED,
      PUBLISHED,
      REFERENCE_UPDATED,
      STATUS_CHANGED,
      UNPUBLISHED,
      UPDATED
    }
    
    
    #region EntitySavedResultDto
    /// <summary>
    /// The result of a mutation
    /// </summary>
    public class EntitySavedResultDto {
      #region members
      /// <summary>
      /// The new version of the item.
      /// </summary>
      [JsonProperty("version")]
      public object version { get; set; }
      #endregion
    }
    #endregion
    
    #region Pages
    /// <summary>
    /// The structure of a pages content type.
    /// </summary>
    public class Pages : Content {
      #region members
      /// <summary>
      /// The timestamp when the object was created.
      /// </summary>
      [JsonProperty("created")]
      public DateTime created { get; set; }
    
      /// <summary>
      /// The user who created the object.
      /// </summary>
      [JsonProperty("createdBy")]
      public string createdBy { get; set; }
    
      /// <summary>
      /// The user who created the object.
      /// </summary>
      [JsonProperty("createdByUser")]
      public User createdByUser { get; set; }
    
      /// <summary>
      /// The data of the content.
      /// </summary>
      [JsonProperty("data")]
      public PagesDataDto data { get; set; }
    
      /// <summary>
      /// The data of the content.
      /// </summary>
      [JsonProperty("data__dynamic")]
      public object dataDynamic { get; set; }
    
      /// <summary>
      /// The edit token.
      /// </summary>
      [JsonProperty("editToken")]
      public string editToken { get; set; }
    
      /// <summary>
      /// The flat data of the content.
      /// </summary>
      [JsonProperty("flatData")]
      public PagesFlatDataDto flatData { get; set; }
    
      /// <summary>
      /// The ID of the object (usually GUID).
      /// </summary>
      [JsonProperty("id")]
      public string id { get; set; }
    
      /// <summary>
      /// The timestamp when the object was updated the last time.
      /// </summary>
      [JsonProperty("lastModified")]
      public DateTime lastModified { get; set; }
    
      /// <summary>
      /// The user who updated the object the last time.
      /// </summary>
      [JsonProperty("lastModifiedBy")]
      public string lastModifiedBy { get; set; }
    
      /// <summary>
      /// The user who updated the object the last time.
      /// </summary>
      [JsonProperty("lastModifiedByUser")]
      public User lastModifiedByUser { get; set; }
    
      /// <summary>
      /// The new status of the content.
      /// </summary>
      [JsonProperty("newStatus")]
      public string newStatus { get; set; }
    
      /// <summary>
      /// The status color of the content.
      /// </summary>
      [JsonProperty("newStatusColor")]
      public string newStatusColor { get; set; }
    
      /// <summary>
      /// The status of the content.
      /// </summary>
      [JsonProperty("status")]
      public string status { get; set; }
    
      /// <summary>
      /// The status color of the content.
      /// </summary>
      [JsonProperty("statusColor")]
      public string statusColor { get; set; }
    
      /// <summary>
      /// The URL to the content.
      /// </summary>
      [JsonProperty("url")]
      public string url { get; set; }
    
      /// <summary>
      /// The version of the objec.
      /// </summary>
      [JsonProperty("version")]
      public int version { get; set; }
      #endregion
    }
    #endregion
    
    #region PagesDataDto
    /// <summary>
    /// The structure of the pages data type.
    /// </summary>
    public class PagesDataDto {
      #region members
      [JsonProperty("slug")]
      public PagesDataSlugDto slug { get; set; }
    
      [JsonProperty("text")]
      public PagesDataTextDto text { get; set; }
    
      [JsonProperty("title")]
      public PagesDataTitleDto title { get; set; }
      #endregion
    }
    #endregion
    
    #region PagesDataInputDto
    /// <summary>
    /// The structure of the pages data input type.
    /// </summary>
    public class PagesDataInputDto {
      #region members
      public PagesDataSlugInputDto slug { get; set; }
    
      public PagesDataTextInputDto text { get; set; }
    
      public PagesDataTitleInputDto title { get; set; }
      #endregion
    
      #region methods
      public dynamic GetInputObject()
      {
        IDictionary<string, object> d = new System.Dynamic.ExpandoObject();
    
        var properties = GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        foreach (var propertyInfo in properties)
        {
          var value = propertyInfo.GetValue(this);
          var defaultValue = propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;
    
          var requiredProp = propertyInfo.GetCustomAttributes(typeof(JsonRequiredAttribute), false).Length > 0;
    
          if (requiredProp || value != defaultValue)
          {
            d[propertyInfo.Name] = value;
          }
        }
        return d;
      }
      #endregion
    }
    #endregion
    
    #region PagesDataSlugDto
    /// <summary>
    /// The structure of the Slug field of the pages content type.
    /// </summary>
    public class PagesDataSlugDto {
      #region members
      [JsonProperty("iv")]
      public string iv { get; set; }
      #endregion
    }
    #endregion
    
    #region PagesDataSlugInputDto
    /// <summary>
    /// The structure of the Slug field of the pages content input type.
    /// </summary>
    public class PagesDataSlugInputDto {
      #region members
      public string iv { get; set; }
      #endregion
    
      #region methods
      public dynamic GetInputObject()
      {
        IDictionary<string, object> d = new System.Dynamic.ExpandoObject();
    
        var properties = GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        foreach (var propertyInfo in properties)
        {
          var value = propertyInfo.GetValue(this);
          var defaultValue = propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;
    
          var requiredProp = propertyInfo.GetCustomAttributes(typeof(JsonRequiredAttribute), false).Length > 0;
    
          if (requiredProp || value != defaultValue)
          {
            d[propertyInfo.Name] = value;
          }
        }
        return d;
      }
      #endregion
    }
    #endregion
    
    #region PagesDataTextDto
    /// <summary>
    /// The structure of the Text field of the pages content type.
    /// </summary>
    public class PagesDataTextDto {
      #region members
      [JsonProperty("iv")]
      public string iv { get; set; }
      #endregion
    }
    #endregion
    
    #region PagesDataTextInputDto
    /// <summary>
    /// The structure of the Text field of the pages content input type.
    /// </summary>
    public class PagesDataTextInputDto {
      #region members
      public string iv { get; set; }
      #endregion
    
      #region methods
      public dynamic GetInputObject()
      {
        IDictionary<string, object> d = new System.Dynamic.ExpandoObject();
    
        var properties = GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        foreach (var propertyInfo in properties)
        {
          var value = propertyInfo.GetValue(this);
          var defaultValue = propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;
    
          var requiredProp = propertyInfo.GetCustomAttributes(typeof(JsonRequiredAttribute), false).Length > 0;
    
          if (requiredProp || value != defaultValue)
          {
            d[propertyInfo.Name] = value;
          }
        }
        return d;
      }
      #endregion
    }
    #endregion
    
    #region PagesDataTitleDto
    /// <summary>
    /// The structure of the Title field of the pages content type.
    /// </summary>
    public class PagesDataTitleDto {
      #region members
      [JsonProperty("iv")]
      public string iv { get; set; }
      #endregion
    }
    #endregion
    
    #region PagesDataTitleInputDto
    /// <summary>
    /// The structure of the Title field of the pages content input type.
    /// </summary>
    public class PagesDataTitleInputDto {
      #region members
      public string iv { get; set; }
      #endregion
    
      #region methods
      public dynamic GetInputObject()
      {
        IDictionary<string, object> d = new System.Dynamic.ExpandoObject();
    
        var properties = GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        foreach (var propertyInfo in properties)
        {
          var value = propertyInfo.GetValue(this);
          var defaultValue = propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;
    
          var requiredProp = propertyInfo.GetCustomAttributes(typeof(JsonRequiredAttribute), false).Length > 0;
    
          if (requiredProp || value != defaultValue)
          {
            d[propertyInfo.Name] = value;
          }
        }
        return d;
      }
      #endregion
    }
    #endregion
    
    #region PagesFlatDataDto
    /// <summary>
    /// The structure of the flat pages data type.
    /// </summary>
    public class PagesFlatDataDto {
      #region members
      [JsonProperty("slug")]
      public string slug { get; set; }
    
      [JsonProperty("text")]
      public string text { get; set; }
    
      [JsonProperty("title")]
      public string title { get; set; }
      #endregion
    }
    #endregion
    
    #region PagesResultDto
    /// <summary>
    /// List of pages items and total count.
    /// </summary>
    public class PagesResultDto {
      #region members
      /// <summary>
      /// The contents.
      /// </summary>
      [JsonProperty("items")]
      public List<Pages> items { get; set; }
    
      /// <summary>
      /// The total count of  contents.
      /// </summary>
      [JsonProperty("total")]
      public int total { get; set; }
      #endregion
    }
    #endregion
    
    #region Posts
    /// <summary>
    /// The structure of a posts content type.
    /// </summary>
    public class Posts : Content {
      #region members
      /// <summary>
      /// The timestamp when the object was created.
      /// </summary>
      [JsonProperty("created")]
      public DateTime created { get; set; }
    
      /// <summary>
      /// The user who created the object.
      /// </summary>
      [JsonProperty("createdBy")]
      public string createdBy { get; set; }
    
      /// <summary>
      /// The user who created the object.
      /// </summary>
      [JsonProperty("createdByUser")]
      public User createdByUser { get; set; }
    
      /// <summary>
      /// The data of the content.
      /// </summary>
      [JsonProperty("data")]
      public PostsDataDto data { get; set; }
    
      /// <summary>
      /// The data of the content.
      /// </summary>
      [JsonProperty("data__dynamic")]
      public object dataDynamic { get; set; }
    
      /// <summary>
      /// The edit token.
      /// </summary>
      [JsonProperty("editToken")]
      public string editToken { get; set; }
    
      /// <summary>
      /// The flat data of the content.
      /// </summary>
      [JsonProperty("flatData")]
      public PostsFlatDataDto flatData { get; set; }
    
      /// <summary>
      /// The ID of the object (usually GUID).
      /// </summary>
      [JsonProperty("id")]
      public string id { get; set; }
    
      /// <summary>
      /// The timestamp when the object was updated the last time.
      /// </summary>
      [JsonProperty("lastModified")]
      public DateTime lastModified { get; set; }
    
      /// <summary>
      /// The user who updated the object the last time.
      /// </summary>
      [JsonProperty("lastModifiedBy")]
      public string lastModifiedBy { get; set; }
    
      /// <summary>
      /// The user who updated the object the last time.
      /// </summary>
      [JsonProperty("lastModifiedByUser")]
      public User lastModifiedByUser { get; set; }
    
      /// <summary>
      /// The new status of the content.
      /// </summary>
      [JsonProperty("newStatus")]
      public string newStatus { get; set; }
    
      /// <summary>
      /// The status color of the content.
      /// </summary>
      [JsonProperty("newStatusColor")]
      public string newStatusColor { get; set; }
    
      /// <summary>
      /// The status of the content.
      /// </summary>
      [JsonProperty("status")]
      public string status { get; set; }
    
      /// <summary>
      /// The status color of the content.
      /// </summary>
      [JsonProperty("statusColor")]
      public string statusColor { get; set; }
    
      /// <summary>
      /// The URL to the content.
      /// </summary>
      [JsonProperty("url")]
      public string url { get; set; }
    
      /// <summary>
      /// The version of the objec.
      /// </summary>
      [JsonProperty("version")]
      public int version { get; set; }
      #endregion
    }
    #endregion
    
    #region PostsDataDto
    /// <summary>
    /// The structure of the posts data type.
    /// </summary>
    public class PostsDataDto {
      #region members
      [JsonProperty("slug")]
      public PostsDataSlugDto slug { get; set; }
    
      [JsonProperty("text")]
      public PostsDataTextDto text { get; set; }
    
      [JsonProperty("title")]
      public PostsDataTitleDto title { get; set; }
      #endregion
    }
    #endregion
    
    #region PostsDataInputDto
    /// <summary>
    /// The structure of the posts data input type.
    /// </summary>
    public class PostsDataInputDto {
      #region members
      public PostsDataSlugInputDto slug { get; set; }
    
      public PostsDataTextInputDto text { get; set; }
    
      public PostsDataTitleInputDto title { get; set; }
      #endregion
    
      #region methods
      public dynamic GetInputObject()
      {
        IDictionary<string, object> d = new System.Dynamic.ExpandoObject();
    
        var properties = GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        foreach (var propertyInfo in properties)
        {
          var value = propertyInfo.GetValue(this);
          var defaultValue = propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;
    
          var requiredProp = propertyInfo.GetCustomAttributes(typeof(JsonRequiredAttribute), false).Length > 0;
    
          if (requiredProp || value != defaultValue)
          {
            d[propertyInfo.Name] = value;
          }
        }
        return d;
      }
      #endregion
    }
    #endregion
    
    #region PostsDataSlugDto
    /// <summary>
    /// The structure of the Slug field of the posts content type.
    /// </summary>
    public class PostsDataSlugDto {
      #region members
      [JsonProperty("iv")]
      public string iv { get; set; }
      #endregion
    }
    #endregion
    
    #region PostsDataSlugInputDto
    /// <summary>
    /// The structure of the Slug field of the posts content input type.
    /// </summary>
    public class PostsDataSlugInputDto {
      #region members
      public string iv { get; set; }
      #endregion
    
      #region methods
      public dynamic GetInputObject()
      {
        IDictionary<string, object> d = new System.Dynamic.ExpandoObject();
    
        var properties = GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        foreach (var propertyInfo in properties)
        {
          var value = propertyInfo.GetValue(this);
          var defaultValue = propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;
    
          var requiredProp = propertyInfo.GetCustomAttributes(typeof(JsonRequiredAttribute), false).Length > 0;
    
          if (requiredProp || value != defaultValue)
          {
            d[propertyInfo.Name] = value;
          }
        }
        return d;
      }
      #endregion
    }
    #endregion
    
    #region PostsDataTextDto
    /// <summary>
    /// The structure of the Text field of the posts content type.
    /// </summary>
    public class PostsDataTextDto {
      #region members
      [JsonProperty("iv")]
      public string iv { get; set; }
      #endregion
    }
    #endregion
    
    #region PostsDataTextInputDto
    /// <summary>
    /// The structure of the Text field of the posts content input type.
    /// </summary>
    public class PostsDataTextInputDto {
      #region members
      public string iv { get; set; }
      #endregion
    
      #region methods
      public dynamic GetInputObject()
      {
        IDictionary<string, object> d = new System.Dynamic.ExpandoObject();
    
        var properties = GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        foreach (var propertyInfo in properties)
        {
          var value = propertyInfo.GetValue(this);
          var defaultValue = propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;
    
          var requiredProp = propertyInfo.GetCustomAttributes(typeof(JsonRequiredAttribute), false).Length > 0;
    
          if (requiredProp || value != defaultValue)
          {
            d[propertyInfo.Name] = value;
          }
        }
        return d;
      }
      #endregion
    }
    #endregion
    
    #region PostsDataTitleDto
    /// <summary>
    /// The structure of the Title field of the posts content type.
    /// </summary>
    public class PostsDataTitleDto {
      #region members
      [JsonProperty("iv")]
      public string iv { get; set; }
      #endregion
    }
    #endregion
    
    #region PostsDataTitleInputDto
    /// <summary>
    /// The structure of the Title field of the posts content input type.
    /// </summary>
    public class PostsDataTitleInputDto {
      #region members
      public string iv { get; set; }
      #endregion
    
      #region methods
      public dynamic GetInputObject()
      {
        IDictionary<string, object> d = new System.Dynamic.ExpandoObject();
    
        var properties = GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        foreach (var propertyInfo in properties)
        {
          var value = propertyInfo.GetValue(this);
          var defaultValue = propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;
    
          var requiredProp = propertyInfo.GetCustomAttributes(typeof(JsonRequiredAttribute), false).Length > 0;
    
          if (requiredProp || value != defaultValue)
          {
            d[propertyInfo.Name] = value;
          }
        }
        return d;
      }
      #endregion
    }
    #endregion
    
    #region PostsFlatDataDto
    /// <summary>
    /// The structure of the flat posts data type.
    /// </summary>
    public class PostsFlatDataDto {
      #region members
      [JsonProperty("slug")]
      public string slug { get; set; }
    
      [JsonProperty("text")]
      public string text { get; set; }
    
      [JsonProperty("title")]
      public string title { get; set; }
      #endregion
    }
    #endregion
    
    #region PostsResultDto
    /// <summary>
    /// List of posts items and total count.
    /// </summary>
    public class PostsResultDto {
      #region members
      /// <summary>
      /// The contents.
      /// </summary>
      [JsonProperty("items")]
      public List<Posts> items { get; set; }
    
      /// <summary>
      /// The total count of  contents.
      /// </summary>
      [JsonProperty("total")]
      public int total { get; set; }
      #endregion
    }
    #endregion
    
    #region User
    /// <summary>
    /// A user that created or modified a content or asset.
    /// </summary>
    public class User {
      #region members
      /// <summary>
      /// The display name of this user.
      /// </summary>
      [JsonProperty("displayName")]
      public string displayName { get; set; }
    
      /// <summary>
      /// The email address ofthis  user.
      /// </summary>
      [JsonProperty("email")]
      public string email { get; set; }
    
      /// <summary>
      /// The ID of this user.
      /// </summary>
      [JsonProperty("id")]
      public string id { get; set; }
      #endregion
    }
    #endregion
  }
  
}
