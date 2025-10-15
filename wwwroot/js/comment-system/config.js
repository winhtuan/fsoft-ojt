// ============================================================
// CONFIGURATION
// ============================================================

export class CommentConfig {
  constructor() {
    this.elements = this.initializeElements();
    this.settings = this.initializeSettings();
  }

  initializeElements() {
    return {
      commentSection: document.querySelector("section[data-plant-id]"),
      commentList: document.getElementById("commentList"),
      commentTemplate: document.getElementById("commentTemplate"),
      btnSubmitComment: document.getElementById("btnSubmitComment"),
      commentContent: document.getElementById("commentContent"),
      commentError: document.getElementById("commentError"),
    };
  }

  initializeSettings() {
    const { commentSection } = this.elements;
    return {
      currentUserId: window.currentUserId,
      plantId: commentSection?.dataset.plantId,
      antiForgeryToken: document.querySelector(
        'input[name="__RequestVerificationToken"]'
      )?.value,
      isAuthenticated:
        window.currentUserId != null &&
        window.currentUserId !== "" &&
        window.currentUserId !== undefined,
    };
  }

  isValid() {
    return this.elements.commentSection !== null;
  }
}
