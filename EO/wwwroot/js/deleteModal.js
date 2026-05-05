function loadDeleteModalCSS() {
  if (document.getElementById("delete-modal-css")) return;

  const link = document.createElement("link");
  link.id = "delete-modal-css";
  link.rel = "stylesheet";
  link.href = "/css/deleteModal.css";

  document.head.appendChild(link);
}


function showDeleteModal({ message, onConfirm, onCancel } = {}) {
  loadDeleteModalCSS();
  return new Promise((resolve) => {
    // Overlay
    const overlay = document.createElement('div');
    overlay.setAttribute('role', 'dialog');
    overlay.setAttribute('aria-modal', 'true');
    overlay.setAttribute('aria-labelledby', 'dm-title');
    overlay.setAttribute('aria-describedby', 'dm-desc');
    overlay.style.cssText = `
      position: fixed; inset: 0; z-index: 9999;
      background: rgba(0,0,0,0.45);
      display: flex; align-items: center; justify-content: center;
      font-family: system-ui, sans-serif;
    `;

 
    overlay.innerHTML = `
  <div class="dm-container">
    
  <div class="dm-icon">
      <svg width="26" height="26" viewBox="0 0 24 24" fill="none">
            <path d="M10.29 3.86L1.82 18a2 2 0 001.71 3h16.94a2 2 0 001.71-3L13.71 3.86a2 2 0 00-3.42 0z"
              fill="#FCEBEB" stroke="#E24B4A" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
            <line x1="12" y1="9" x2="12" y2="13" stroke="#E24B4A" stroke-width="1.5" stroke-linecap="round"/>
            <circle cx="12" cy="17" r="0.8" fill="#E24B4A"/>
          </svg>
</div>

    <p class="dm-title">Delete Project</p>

    <p class="dm-desc">
      ${message || 'This action is permanent and cannot be undone.'}
    </p>

    <div class="dm-divider"></div>

    <div class="dm-actions">
      <button id="dm-cancel" class="dm-btn dm-cancel">
        No, keep it.
      </button>

      <button id="dm-confirm" class="dm-btn dm-danger">
        Yes, delete!
      </button>
    </div>

  </div>
`;

    const finish = (confirmed) => {
      document.body.removeChild(overlay);
      document.removeEventListener('keydown', onKey);
      resolve(confirmed);
      confirmed ? onConfirm?.() : onCancel?.();
    };

    const onKey = (e) => { if (e.key === 'Escape') finish(false); };
    document.addEventListener('keydown', onKey);

    overlay.addEventListener('click', (e) => { if (e.target === overlay) finish(false); });
    overlay.querySelector('#dm-cancel').addEventListener('click', () => finish(false));
    overlay.querySelector('#dm-confirm').addEventListener('click', () => finish(true));

    document.body.appendChild(overlay);
    overlay.querySelector('#dm-cancel').focus(); // focus the safe option first
  });
}