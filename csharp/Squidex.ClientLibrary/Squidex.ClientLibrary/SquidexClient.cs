// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Concurrent;
using Squidex.ClientLibrary.Utils;

namespace Squidex.ClientLibrary;

/// <summary>
/// Default implementation of the <see cref="ISquidexClient"/> interface.
/// </summary>
/// <seealso cref="ISquidexClient" />
public sealed class SquidexClient : ISquidexClient
{
    private readonly ConcurrentDictionary<(string, Type, Type), object> contentsClient = new ConcurrentDictionary<(string, Type, Type), object>();
    private readonly ConcurrentDictionary<string, object> dynamicContentsClient = new ConcurrentDictionary<string, object>();
    private readonly ConcurrentDictionary<(Type, Type), object> sharedContentsClient = new ConcurrentDictionary<(Type, Type), object>();
    private IAppsClient appsClient;
    private IAssetsClient assetsClient;
    private IBackupsClient backupsClient;
    private IContentsSharedClient<DynamicContent, DynamicData> contentsSharedClient;
    private IDiagnosticsClient diagnosticsClient;
    private IEventConsumersClient eventConsumersClient;
    private IExtendableRulesClient extendableRulesClient;
    private IHistoryClient historyClient;
    private IJobsClient jobsCLient;
    private ILanguagesClient languagesClient;
    private IPingClient pingClient;
    private IPlansClient plansClient;
    private IRulesClient rulesClient;
    private ISchemasClient schemasClient;
    private ISearchClient searchClient;
    private IStatisticsClient statisticsClient;
    private ITeamsClient teamsClient;
    private ITemplatesClient templatesClient;
    private ITranslationsClient translationsClient;
    private IUserManagementClient userManagementClient;
    private IUsersClient usersClient;

    /// <inheritdoc/>
    public SquidexOptions Options { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SquidexClient"/> class with the options.
    /// </summary>
    /// <param name="options">The options. Cannot be null.</param>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
    public SquidexClient(SquidexOptions options)
    {
        Guard.NotNull(options, nameof(options));

        options.CheckAndFreeze();
        Options = options;
    }

    /// <inheritdoc/>
    public string? GenerateImageUrl(string? id)
    {
        if (id == null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(Options.AssetCDN))
        {
            return GenerateUrl(Options.AssetCDN, id);
        }

        return GenerateUrl(Options.Url, $"api/assets/{id}");
    }

    /// <inheritdoc/>
    public string? GenerateImageUrl(IEnumerable<string>? id)
    {
        return GenerateImageUrl(id?.FirstOrDefault());
    }

    /// <inheritdoc />
    public string? GenerateUrl(string? relativeUrl)
    {
        return GenerateUrl(Options.Url, relativeUrl);
    }

    /// <inheritdoc />
    public string? GenerateAssetCDNUrl(string relativeUrl)
    {
        return GenerateUrl(Options.AssetCDN, relativeUrl);
    }

    /// <inheritdoc />
    public string? GenerateContentCDNUrl(string relativeUrl)
    {
        return GenerateUrl(Options.ContentCDN, relativeUrl);
    }

    private static string? GenerateUrl(string baseUrl, string? relativeUrl)
    {
        if (relativeUrl == null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException("URL is not configured.");
        }

        return $"{baseUrl}{relativeUrl.TrimStart('/')}";
    }

    /// <inheritdoc/>
    public IAppsClient Apps
    {
        get => appsClient ??= new AppsClient(Options);
    }

    /// <inheritdoc/>
    public IAssetsClient Assets
    {
        get => assetsClient ??= new AssetsClient(Options);
    }

    /// <inheritdoc/>
    public IBackupsClient Backups
    {
        get => backupsClient ??= new BackupsClient(Options);
    }

    /// <inheritdoc/>
    public IContentsSharedClient<DynamicContent, DynamicData> SharedDynamicContents
    {
        get => contentsSharedClient ??= new ContentsSharedClient<DynamicContent, DynamicData>(Options);
    }

    /// <inheritdoc/>
    public IDiagnosticsClient Diagnostics
    {
        get => diagnosticsClient ??= new DiagnosticsClient(Options);
    }

    /// <inheritdoc/>
    public IEventConsumersClient EventConsumers
    {
        get => eventConsumersClient ??= new EventConsumersClient(Options);
    }

    /// <inheritdoc/>
    public IExtendableRulesClient ExtendableRules
    {
        get => extendableRulesClient ??= new ExtendableRulesClient(Options);
    }

    /// <inheritdoc/>
    public IHistoryClient History
    {
        get => historyClient ??= new HistoryClient(Options);
    }

    /// <inheritdoc/>
    public IJobsClient Jobs
    {
        get => jobsCLient ??= new JobsClient(Options);
    }

    /// <inheritdoc/>
    public ILanguagesClient Languages
    {
        get => languagesClient ??= new LanguagesClient(Options);
    }

    /// <inheritdoc/>
    public IPingClient Ping
    {
        get => pingClient ??= new PingClient(Options);
    }

    /// <inheritdoc/>
    public IPlansClient Plans
    {
        get => plansClient ??= new PlansClient(Options);
    }

    /// <inheritdoc/>
    public IRulesClient Rules
    {
        get => rulesClient ??= new RulesClient(Options);
    }

    /// <inheritdoc/>
    public ISchemasClient Schemas
    {
        get => schemasClient ??= new SchemasClient(Options);
    }

    /// <inheritdoc/>
    public ISearchClient Search
    {
        get => searchClient ??= new SearchClient(Options);
    }

    /// <inheritdoc/>
    public IStatisticsClient Statistics
    {
        get => statisticsClient ??= new StatisticsClient(Options);
    }

    /// <inheritdoc/>
    public ITeamsClient Teams
    {
        get => teamsClient ??= new TeamsClient(Options);
    }

    /// <inheritdoc/>
    public ITemplatesClient Templates
    {
        get => templatesClient ??= new TemplatesClient(Options);
    }

    /// <inheritdoc/>
    public ITranslationsClient Translations
    {
        get => translationsClient ??= new TranslationsClient(Options);
    }

    /// <inheritdoc/>
    public IUsersClient Users
    {
        get => usersClient ??= new UsersClient(Options);
    }

    /// <inheritdoc/>
    public IUserManagementClient UserManagement
    {
        get => userManagementClient ??= new UserManagementClient(Options);
    }

    /// <inheritdoc/>
    public IContentsClient<TEntity, TData> Contents<TEntity, TData>(string schemaName) where TEntity : Content<TData> where TData : class, new()
    {
        return (IContentsClient<TEntity, TData>)contentsClient.GetOrAdd((schemaName, typeof(TEntity), typeof(TData)), k =>
            new ContentsClient<TEntity, TData>(Options, k.Item1));
    }

    /// <inheritdoc/>
    public IContentsClient<DynamicContent, DynamicData> DynamicContents(string schemaName)
    {
        return (IContentsClient<DynamicContent, DynamicData>)dynamicContentsClient.GetOrAdd(schemaName, k =>
            new ContentsClient<DynamicContent, DynamicData>(Options, k));
    }

    /// <inheritdoc/>
    public IContentsSharedClient<TEntity, TData> SharedContents<TEntity, TData>() where TEntity : Content<TData> where TData : class, new()
    {
        return (IContentsSharedClient<TEntity, TData>)sharedContentsClient.GetOrAdd((typeof(TEntity), typeof(TData)), k =>
            new ContentsSharedClient<TEntity, TData>(Options));
    }

    /// <inheritdoc/>
    public HttpClient CreateHttpClient()
    {
        return Options.ClientProvider.Get();
    }
}
