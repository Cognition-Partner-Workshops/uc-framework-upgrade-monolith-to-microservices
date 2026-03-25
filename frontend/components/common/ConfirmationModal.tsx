import React from "react";

interface ConfirmationModalProps {
  isOpen: boolean;
  title: string;
  message: string;
  onConfirm: () => void;
  onCancel: () => void;
  confirmText?: string;
  cancelText?: string;
}

const overlayStyle: React.CSSProperties = {
  position: "fixed",
  top: 0,
  left: 0,
  right: 0,
  bottom: 0,
  backgroundColor: "rgba(0, 0, 0, 0.5)",
  display: "flex",
  alignItems: "center",
  justifyContent: "center",
  zIndex: 9999,
};

const modalStyle: React.CSSProperties = {
  backgroundColor: "#fff",
  borderRadius: "8px",
  padding: "24px",
  maxWidth: "440px",
  width: "90%",
  boxShadow: "0 4px 20px rgba(0, 0, 0, 0.15)",
};

const titleStyle: React.CSSProperties = {
  margin: "0 0 12px 0",
  fontSize: "20px",
  fontWeight: 700,
  color: "#333",
};

const messageStyle: React.CSSProperties = {
  margin: "0 0 24px 0",
  fontSize: "15px",
  color: "#555",
  lineHeight: 1.5,
};

const buttonContainerStyle: React.CSSProperties = {
  display: "flex",
  justifyContent: "flex-end",
  gap: "12px",
};

const cancelButtonStyle: React.CSSProperties = {
  padding: "8px 20px",
  fontSize: "14px",
  border: "1px solid #ccc",
  borderRadius: "4px",
  backgroundColor: "#fff",
  color: "#333",
  cursor: "pointer",
};

const confirmButtonStyle: React.CSSProperties = {
  padding: "8px 20px",
  fontSize: "14px",
  border: "1px solid #b85c5c",
  borderRadius: "4px",
  backgroundColor: "#b85c5c",
  color: "#fff",
  cursor: "pointer",
};

class ConfirmationModal extends React.Component<ConfirmationModalProps> {
  private modalRef: React.RefObject<HTMLDivElement>;
  private previousActiveElement: Element | null;

  constructor(props: ConfirmationModalProps) {
    super(props);
    this.modalRef = React.createRef();
    this.previousActiveElement = null;
    this.handleKeyDown = this.handleKeyDown.bind(this);
    this.handleOverlayClick = this.handleOverlayClick.bind(this);
  }

  componentDidUpdate(prevProps: ConfirmationModalProps) {
    if (!prevProps.isOpen && this.props.isOpen) {
      this.previousActiveElement = document.activeElement;
      document.addEventListener("keydown", this.handleKeyDown);
      setTimeout(() => {
        if (this.modalRef.current) {
          this.modalRef.current.focus();
        }
      }, 0);
    } else if (prevProps.isOpen && !this.props.isOpen) {
      document.removeEventListener("keydown", this.handleKeyDown);
      if (
        this.previousActiveElement &&
        (this.previousActiveElement as HTMLElement).focus
      ) {
        (this.previousActiveElement as HTMLElement).focus();
      }
    }
  }

  componentWillUnmount() {
    document.removeEventListener("keydown", this.handleKeyDown);
  }

  handleKeyDown(e: KeyboardEvent) {
    if (e.key === "Escape") {
      this.props.onCancel();
      return;
    }

    if (e.key === "Tab" && this.modalRef.current) {
      const focusable = this.modalRef.current.querySelectorAll(
        'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
      );
      if (focusable.length === 0) return;

      const first = focusable[0] as HTMLElement;
      const last = focusable[focusable.length - 1] as HTMLElement;

      if (e.shiftKey) {
        if (document.activeElement === first) {
          e.preventDefault();
          last.focus();
        }
      } else {
        if (document.activeElement === last) {
          e.preventDefault();
          first.focus();
        }
      }
    }
  }

  handleOverlayClick(e: React.MouseEvent<HTMLDivElement>) {
    if (e.target === e.currentTarget) {
      this.props.onCancel();
    }
  }

  render() {
    const {
      isOpen,
      title,
      message,
      onConfirm,
      onCancel,
      confirmText = "Confirm",
      cancelText = "Cancel",
    } = this.props;

    if (!isOpen) return null;

    return (
      <div
        style={overlayStyle}
        onClick={this.handleOverlayClick}
        role="dialog"
        aria-modal="true"
        aria-labelledby="confirmation-modal-title"
      >
        <div
          ref={this.modalRef}
          style={modalStyle}
          tabIndex={-1}
        >
          <h2 id="confirmation-modal-title" style={titleStyle}>
            {title}
          </h2>
          <p style={messageStyle}>{message}</p>
          <div style={buttonContainerStyle}>
            <button style={cancelButtonStyle} onClick={onCancel}>
              {cancelText}
            </button>
            <button style={confirmButtonStyle} onClick={onConfirm}>
              {confirmText}
            </button>
          </div>
        </div>
      </div>
    );
  }
}

export default ConfirmationModal;
