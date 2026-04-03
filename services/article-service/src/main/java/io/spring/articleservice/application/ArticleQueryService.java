package io.spring.articleservice.application;

import io.spring.articleservice.application.data.ArticleData;
import io.spring.articleservice.application.data.ArticleDataList;
import io.spring.articleservice.application.data.ArticleFavoriteCount;
import io.spring.articleservice.infrastructure.client.UserServiceClient;
import io.spring.articleservice.infrastructure.mybatis.readservice.ArticleFavoritesReadService;
import io.spring.articleservice.infrastructure.mybatis.readservice.ArticleReadService;
import io.spring.shared.data.ProfileData;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Optional;
import java.util.Set;
import java.util.stream.Collectors;
import lombok.AllArgsConstructor;
import org.springframework.stereotype.Service;

@Service
@AllArgsConstructor
public class ArticleQueryService {
  private ArticleReadService articleReadService;
  private ArticleFavoritesReadService articleFavoritesReadService;
  private UserServiceClient userServiceClient;

  public Optional<ArticleData> findById(String id, String currentUserId) {
    ArticleData articleData = articleReadService.findById(id);
    if (articleData == null) {
      return Optional.empty();
    }
    return Optional.of(setArticleExtra(articleData, currentUserId));
  }

  public Optional<ArticleData> findBySlug(String slug, String currentUserId) {
    ArticleData articleData = articleReadService.findBySlug(slug);
    if (articleData == null) {
      return Optional.empty();
    }
    return Optional.of(setArticleExtra(articleData, currentUserId));
  }

  public ArticleDataList findRecentArticles(
      String tag, String author, String favoritedBy, Page page, String currentUserId) {
    List<String> articleIds = articleReadService.queryArticles(tag, author, favoritedBy, page);
    int count = articleReadService.countArticle(tag, author, favoritedBy);
    if (articleIds.isEmpty()) {
      return new ArticleDataList(new ArrayList<>(), count);
    }
    List<ArticleData> articles = articleReadService.findArticles(articleIds);
    fillExtraInfo(articles, currentUserId);
    return new ArticleDataList(articles, count);
  }

  public ArticleDataList findUserFeed(String userId, Page page) {
    List<String> followedUsers = userServiceClient.followedUsers(userId);
    if (followedUsers.isEmpty()) {
      return new ArticleDataList(new ArrayList<>(), 0);
    }
    List<String> articleIds = articleReadService.findArticlesOfAuthors(followedUsers, page);
    int count = articleReadService.countFeedSize(followedUsers);
    if (articleIds.isEmpty()) {
      return new ArticleDataList(new ArrayList<>(), count);
    }
    List<ArticleData> articles = articleReadService.findArticles(articleIds);
    fillExtraInfo(articles, userId);
    return new ArticleDataList(articles, count);
  }

  private ArticleData setArticleExtra(ArticleData articleData, String currentUserId) {
    List<ArticleData> list = new ArrayList<>();
    list.add(articleData);
    fillExtraInfo(list, currentUserId);
    return list.get(0);
  }

  private void fillExtraInfo(List<ArticleData> articles, String currentUserId) {
    setFavoriteCount(articles);
    if (currentUserId != null) {
      setIsFavorite(articles, currentUserId);
      setIsFollowingAuthor(articles, currentUserId);
    }
    setAuthorProfiles(articles);
  }

  private void setFavoriteCount(List<ArticleData> articles) {
    List<String> ids = articles.stream().map(ArticleData::getId).collect(Collectors.toList());
    List<ArticleFavoriteCount> favoritesCounts =
        articleFavoritesReadService.articlesFavoriteCount(ids);
    Map<String, Integer> countMap = new HashMap<>();
    for (ArticleFavoriteCount count : favoritesCounts) {
      countMap.put(count.getId(), count.getCount());
    }
    for (ArticleData article : articles) {
      article.setFavoritesCount(countMap.getOrDefault(article.getId(), 0));
    }
  }

  private void setIsFavorite(List<ArticleData> articles, String currentUserId) {
    List<String> ids = articles.stream().map(ArticleData::getId).collect(Collectors.toList());
    Set<String> favorites = articleFavoritesReadService.userFavorites(ids, currentUserId);
    for (ArticleData article : articles) {
      article.setFavorited(favorites.contains(article.getId()));
    }
  }

  private void setIsFollowingAuthor(List<ArticleData> articles, String currentUserId) {
    List<String> authorIds =
        articles.stream()
            .filter(a -> a.getProfileData() != null)
            .map(a -> a.getProfileData().getId())
            .distinct()
            .collect(Collectors.toList());
    if (!authorIds.isEmpty()) {
      Set<String> followingAuthors =
          userServiceClient.followingAuthors(currentUserId, authorIds);
      for (ArticleData article : articles) {
        if (article.getProfileData() != null) {
          article
              .getProfileData()
              .setFollowing(followingAuthors.contains(article.getProfileData().getId()));
        }
      }
    }
  }

  private void setAuthorProfiles(List<ArticleData> articles) {
    // Author profiles are already populated by the SQL join in ArticleReadService
    // This method can enrich with additional data from User Service if needed
  }
}
