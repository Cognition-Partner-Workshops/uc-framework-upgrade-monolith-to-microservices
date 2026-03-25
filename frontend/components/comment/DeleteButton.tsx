import axios from "axios";
import React from "react";
import { useRouter } from "next/router";
import useSWR, { trigger } from "swr";

import ConfirmationModal from "../common/ConfirmationModal";
import { SERVER_BASE_URL } from "../../lib/utils/constant";
import storage from "../../lib/utils/storage";

const DeleteButton = ({ commentId }) => {
  const { data: currentUser } = useSWR("user", storage);
  const router = useRouter();
  const {
    query: { pid },
  } = router;

  const [showDeleteModal, setShowDeleteModal] = React.useState(false);

  const handleDeleteClick = () => {
    setShowDeleteModal(true);
  };

  const handleDeleteConfirm = async () => {
    setShowDeleteModal(false);
    await axios.delete(
      `${SERVER_BASE_URL}/articles/${pid}/comments/${commentId}`,
      {
        headers: {
          Authorization: `Token ${currentUser?.token}`,
        },
      }
    );
    trigger(`${SERVER_BASE_URL}/articles/${pid}/comments`);
  };

  const handleDeleteCancel = () => {
    setShowDeleteModal(false);
  };

  return (
    <span className="mod-options">
      <i className="ion-trash-a" onClick={handleDeleteClick} />
      <ConfirmationModal
        isOpen={showDeleteModal}
        title="Delete Comment"
        message="Are you sure you want to delete this comment?"
        onConfirm={handleDeleteConfirm}
        onCancel={handleDeleteCancel}
        confirmText="Delete"
        cancelText="Cancel"
      />
    </span>
  );
};

export default DeleteButton;
