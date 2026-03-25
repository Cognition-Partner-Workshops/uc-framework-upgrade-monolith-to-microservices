package io.spring.application;

import io.spring.application.data.ArticleData;
import io.spring.application.data.ArticleDataList;
import io.spring.application.data.ArticleFavoriteCount;
import io.spring.application.data.ProfileData;
import io.spring.core.user.User;
import io.spring.infrastructure.mybatis.readservice.ArticleFavoritesReadService;
import io.spring.infrastructure.mybatis.readservice.ArticleReadService;
import io.spring.infrastructure.mybatis.readservice.UserReadService;
import io.spring.infrastructure.mybatis.readservice.UserRelationshipQueryService;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Optional;
import java.util.Set;
import java.util.stream.Collectors;
import lombok.AllArgsConstructor;
import org.joda.time.DateTime;
import org.springframework.stereotype.Service;

@Service
@AllArgsConstructor
public class ArticleQueryService {
  private ArticleReadService articleReadService;
  private UserRelationshipQueryService userRelationshipQueryService;
  private ArticleFavoritesReadService articleFavoritesReadService;
  private UserReadService userReadService;

  public Optional<ArticleData> findById(String id, User user) {
    ArticleData articleData = articleReadService.findById(id);
    if (articleData == null) {
      return Optional.empty();
    } else {
      if (user != null) {
        fillExtraInfo(id, user, articleData);
      }
      return Optional.of(articleData);
    }
  }

  public Optional<ArticleData> findBySlug(String slug, User user) {
    ArticleData articleData = articleReadService.findBySlug(slug);
    if (articleData == null) {
      return Optional.empty();
    } else {
      if (user != null) {
        fillExtraInfo(articleData.getId(), user, articleData);
      }
      return Optional.of(articleData);
    }
  }

  public CursorPager<ArticleData> findRecentArticlesWithCursor(
      String tag,
      String author,
      String favoritedBy,
      CursorPageParameter<DateTime> page,
      User currentUser) {
    List<String> articleIds =
        articleReadService.findArticlesWithCursor(tag, author, favoritedBy, page);
    boolean hasExtra = articleIds.size() > page.getLimit();
    if (hasExtra) {
      articleIds.remove(articleIds.size() - 1);
    }
    if (!articleIds.isEmpty()) {
      List<ArticleData> articles = articleReadService.findArticles(articleIds);
      fillExtraInfo(articles, currentUser);
      return new CursorPager<>(articles, page.getDirection(), hasExtra);
    } else {
      return new CursorPager<>(new ArrayList<>(), page.getDirection(), hasExtra);
    }
  }

  public CursorPager<ArticleData> findUserFeedWithCursor(
      User user, CursorPageParameter<DateTime> page) {
    List<String> followedUsers = userRelationshipQueryService.followedUsers(user.getId());
    if (followedUsers.isEmpty()) {
      return new CursorPager<>(new ArrayList<>(), page.getDirection(), false);
    } else {
      List<ArticleData> articles =
          articleReadService.findArticlesOfAuthorsWithCursor(followedUsers, page);
      boolean hasExtra = articles.size() > page.getLimit();
      if (hasExtra) {
        articles.remove(articles.size() - 1);
      }
      fillExtraInfo(articles, user);
      return new CursorPager<>(articles, page.getDirection(), hasExtra);
    }
  }

  public ArticleDataList findRecentArticles(
      String tag, String author, String favoritedBy, Page page, User currentUser) {
    List<String> articleIds = articleReadService.queryArticles(tag, author, favoritedBy, page);
    int articleCount = articleReadService.countArticle(tag, author, favoritedBy);
    if (articleIds.isEmpty()) {
      return new ArticleDataList(new ArrayList<>(), articleCount);
    } else {
      List<ArticleData> articles = articleReadService.findArticles(articleIds);
      fillExtraInfo(articles, currentUser);
      return new ArticleDataList(articles, articleCount);
    }
  }

  public ArticleDataList findUserFeed(User user, Page page) {
    List<String> followedUsers = userRelationshipQueryService.followedUsers(user.getId());
    if (followedUsers.isEmpty()) {
      return new ArticleDataList(new ArrayList<>(), 0);
    } else {
      List<ArticleData> articles =
          articleReadService.findArticlesOfAuthors(followedUsers, page);
      int articleCount = articleReadService.countFeedSize(followedUsers);
      fillExtraInfo(articles, user);
      return new ArticleDataList(articles, articleCount);
    }
  }

  private void fillExtraInfo(String id, User user, ArticleData articleData) {
    articleData.setFavorited(
        articleFavoritesReadService.isUserFavorite(user.getId(), id));
    articleData.setFavoritesCount(articleFavoritesReadService.articleFavoriteCount(id));
    if (articleData.getProfileData() != null) {
      articleData
          .getProfileData()
          .setFollowing(
              userRelationshipQueryService.isUserFollowing(
                  user.getId(), articleData.getProfileData().getId()));
    }
  }

  private void fillExtraInfo(List<ArticleData> articles, User currentUser) {
    if (articles.isEmpty()) {
      return;
    }
    List<String> ids = articles.stream().map(ArticleData::getId).collect(Collectors.toList());
    List<ArticleFavoriteCount> favoritesCounts =
        articleFavoritesReadService.articlesFavoriteCount(ids);
    Map<String, Integer> countMap = new HashMap<>();
    favoritesCounts.forEach(item -> countMap.put(item.getId(), item.getCount()));
    List<String> authorIds =
        articles.stream()
            .filter(a -> a.getProfileData() != null)
            .map(a -> a.getProfileData().getId())
            .collect(Collectors.toList());
    Set<String> followingAuthors =
        currentUser != null
            ? userRelationshipQueryService.followingAuthors(currentUser.getId(), authorIds)
            : new java.util.HashSet<>();
    Set<String> userFavorites =
        currentUser != null
            ? articleFavoritesReadService.userFavorites(ids, currentUser)
            : new java.util.HashSet<>();
    articles.forEach(
        a -> {
          a.setFavoritesCount(countMap.getOrDefault(a.getId(), 0));
          a.setFavorited(userFavorites.contains(a.getId()));
          if (a.getProfileData() != null) {
            a.getProfileData()
                .setFollowing(followingAuthors.contains(a.getProfileData().getId()));
          }
        });
  }
}
