using System.Text.Json.Serialization;

namespace RealWorld.Application.DTOs;

public class ArticleDataList
{
    [JsonPropertyName("articles")]
    public List<ArticleData> ArticleDatas { get; }

    [JsonPropertyName("articlesCount")]
    public int Count { get; }

    public ArticleDataList(List<ArticleData> articleDatas, int count)
    {
        ArticleDatas = articleDatas;
        Count = count;
    }
}
