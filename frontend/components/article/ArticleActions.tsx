import Router, { useRouter } from "next/router";
import React from "react";
import useSWR, { trigger } from "swr";

import CustomLink from "../common/CustomLink";
import ConfirmationModal from "../common/ConfirmationModal";
import checkLogin from "../../lib/utils/checkLogin";
import ArticleAPI from "../../lib/api/article";
import { SERVER_BASE_URL } from "../../lib/utils/constant";
import storage from "../../lib/utils/storage";
import Maybe from "../common/Maybe";

const ArticleActions = ({ article }) => {
  const { data: currentUser } = useSWR("user", storage);
  const isLoggedIn = checkLogin(currentUser);
  const router = useRouter();
  const {
    query: { pid },
  } = router;

  const [showDeleteModal, setShowDeleteModal] = React.useState(false);

  const handleDeleteClick = () => {
    if (!isLoggedIn) return;
    setShowDeleteModal(true);
  };

  const handleDeleteConfirm = async () => {
    setShowDeleteModal(false);
    await ArticleAPI.delete(pid, currentUser?.token);
    trigger(`${SERVER_BASE_URL}/articles/${pid}`);
    Router.push(`/`);
  };

  const handleDeleteCancel = () => {
    setShowDeleteModal(false);
  };

  const canModify =
    isLoggedIn && currentUser?.username === article?.author?.username;

  return (
    <Maybe test={canModify}>
      <span>
        <CustomLink
          href="/editor/[pid]"
          as={`/editor/${article.slug}`}
          className="btn btn-outline-secondary btn-sm"
        >
          <i className="ion-edit" /> Edit Article
        </CustomLink>

        <button
          className="btn btn-outline-danger btn-sm"
          onClick={handleDeleteClick}
        >
          <i className="ion-trash-a" /> Delete Article
        </button>

        <ConfirmationModal
          isOpen={showDeleteModal}
          title="Delete Article"
          message="Do you really want to delete this article? This action cannot be undone."
          onConfirm={handleDeleteConfirm}
          onCancel={handleDeleteCancel}
          confirmText="Delete"
          cancelText="Cancel"
        />
      </span>
    </Maybe>
  );
};

export default ArticleActions;
