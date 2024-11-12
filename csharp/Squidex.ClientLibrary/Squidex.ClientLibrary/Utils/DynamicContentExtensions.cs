namespace Squidex.ClientLibrary.Utils;

public static class DynamicContentExtensions
{
    public static DynamicData GetDataWithId(this DynamicContent content)
    {
        var data = content.Data;
        data[DynamicData.IdentityField] = content.Id;

        return data;
    }
}
