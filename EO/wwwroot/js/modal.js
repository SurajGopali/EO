function showModal(items, title = "Details") {
    // 1. Set Title & Basic Modal Styles
    const modalTitleEl = document.getElementById("modalTitle");
    modalTitleEl.innerText = title;

    const modalContent = document.querySelector("#globalModal .modal-content");
    const modalBody = document.getElementById("modalBody");
    const modalDialog = document.querySelector("#globalModal .modal-dialog");
    const closeBtn = document.querySelector("#globalModal .btn-close");

    if (modalDialog) {
        modalDialog.classList.add("modal-dialog-centered");
    }

    if (closeBtn) {
        closeBtn.classList.remove("btn-close-white");
        closeBtn.style.setProperty("background-image", `url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 16 16' fill='%23dc3545'%3e%3cpath d='M.293.293a1 1 0 011.414 0L8 6.586 14.293.293a1 1 0 111.414 1.414L9.414 8l6.293 6.293a1 1 0 01-1.414 1.414L8 9.414l-6.293 6.293a1 1 0 01-1.414-1.414L6.586 8 .293 1.707a1 1 0 010-1.414z'/%3e%3c/svg%3e")`, "important");
        closeBtn.style.setProperty("opacity", "1", "important");
    }

    if (modalContent) {
        modalContent.style.setProperty("background-color", "var(--modal-bg, var(--app-bg, #ffffff))" , "important");
        modalContent.style.setProperty("color", "var(--main-text, #2d3436)" ,"important");
        modalContent.style.setProperty("border", "1px solid var(--border-glow, #e9ecef)" , "important");
    }
    
    // Clear padding to let the list items go edge-to-edge if desired
    modalBody.style.padding = "0";

    let html = "";

    if (!Array.isArray(items) || items.length === 0) {
        html = `
            <div class="empty-state" style="padding: 40px; text-align: center; color: var(--sidebar-text, #636e72);">
                No data available
            </div>`;
    } 
    else {
        items.forEach(item => {
            if (!item) return; 

            const name   = item.name  || item.title;
            const role   = item.role  || item.desc;
            const date   = item.date || item.joined;
            const avatar = item.avatar;

            html += `
                <div class="d-flex align-items-center gap-3 p-3" 
                     style="border-bottom: 1px solid var(--border-color, #e9ecef); transition: background 0.2s;">

                    <!-- Avatar -->
                    ${avatar ? `
                        <img src="${avatar}"
                             onerror="this.parentElement.innerHTML='<div style=\'width:48px;height:48px;background:var(--app-bg, #f8f9fa);border-radius:8px;border: 1px solid var(--border-color, #e9ecef);\'></div>'"
                             style="width:48px; height:48px; border-radius:8px; object-fit:cover; border: 1px solid var(--border-color, #e9ecef);" />
                    ` : `
                        <div style="width:48px; height:48px; border-radius:8px; background: var(--app-bg, #f8f9fa); display:flex; align-items:center; justify-content:center; color: var(--sidebar-text, #636e72); border: 1px solid var(--border-color, #e9ecef);">
                            <small style="font-weight:bold;">${name ? name.charAt(0).toUpperCase() : '?'}</small>
                        </div>
                    `}

                    <!-- Text Info -->
                    <div class="flex-grow-1">
                        <div style="font-weight:600; color: var(--main-text, #2d3436); font-size: 1rem;">
                            ${name || 'Unnamed'}
                        </div>
                        ${role ? `
                            <div style="margin-top: 4px;">
                                <span class="badge-role" style="background: var(--app-bg, #f8f9fa); padding: 2px 8px; border-radius: 4px; font-size: 0.8rem; border: 1px solid var(--border-color, #e9ecef); color: var(--sidebar-text, #636e72);">
                                    ${role}
                                </span>
                            </div>
                        ` : ""}
                    </div>

                    <!-- Date -->
                    ${date ? `
                        <div style="font-size:0.75rem; color: var(--sidebar-text, #888); white-space:nowrap; align-self: flex-start;">
                            ${date}
                        </div>
                    ` : ""}
                </div>
            `;
        });
    }

    modalBody.innerHTML = html;

    const modalElement = document.getElementById("globalModal");
    const modalInstance = bootstrap.Modal.getOrCreateInstance(modalElement);
    modalInstance.show();
}