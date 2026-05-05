/*!
 * toast.js — lightweight success/error toast utility
 * Drop this file in wwwroot/js/ and include it in your layout or page.
 * Usage:
 *   Toast.success("Profile saved!", "Transaction ID: #14402");
 *   Toast.error("Something went wrong");
 */

(function (global) {

  function injectStyles() {
    if (document.getElementById("snk-toast-styles")) return;
    var style = document.createElement("style");
    style.id = "snk-toast-styles";
   style.textContent = [
  "@import url('https://fonts.googleapis.com/css2?family=DM+Sans:wght@400;500;600&display=swap');",

  /* STACK */
  ".snk-stack{",
    "position:fixed;top:24px;right:24px;z-index:99999;",
    "display:flex;flex-direction:column;gap:10px;",
    "pointer-events:none;",
  "}",

  /* TOAST BASE */
  ".snk-toast{",
    "font-family:'DM Sans',sans-serif;",
    "pointer-events:all;",
    "display:inline-flex;align-items:center;gap:12px;",
    "padding:13px 18px 13px 14px;",
    "border-radius:999px;",
    "position:relative;overflow:hidden;",
    "width:max-content;max-width:350px;",
    "opacity:0;transform:translateX(48px) scale(0.94);",
    "animation:snkIn 0.38s cubic-bezier(0.22,1,0.36,1) forwards;",
  "}",

  /* SUCCESS */
  ".snk-toast.snk-success{",
    "background:var(--toast-success-bg);",
    "border:1px solid var(--toast-success-border);",
    "box-shadow:var(--shadow-sm);",
  "}",

  /* ERROR */
  ".snk-toast.snk-error{",
    "background:var(--toast-error-bg);",
    "border:1px solid var(--toast-error-border);",
    "box-shadow:var(--shadow-sm);",
  "}",

  /* ICON */
  ".snk-icon{",
    "flex-shrink:0;width:24px;height:24px;border-radius:50%;",
    "display:flex;align-items:center;justify-content:center;",
  "}",

  ".snk-success .snk-icon{",
    "background:color-mix(in srgb, var(--toast-icon-success) 15%, transparent);",
    "animation:snkPulseGreen 2.6s ease-in-out 0.6s infinite;",
  "}",

  ".snk-error .snk-icon{",
    "background:color-mix(in srgb, var(--toast-icon-error) 15%, transparent);",
    "animation:snkPulseRed 2.6s ease-in-out 0.6s infinite;",
  "}",

  ".snk-icon svg{width:13px;height:13px;stroke-width:2.6;stroke-linecap:round;stroke-linejoin:round;fill:none;}",

  ".snk-success .snk-icon svg{stroke:var(--toast-icon-success);}",
  ".snk-error   .snk-icon svg{stroke:var(--toast-icon-error);}",

  /* TEXT */
  ".snk-text{display:flex;flex-direction:column;gap:1px;flex:1;min-width:0;}",

  ".snk-title{",
    "font-size:13.5px;font-weight:600;line-height:1.3;letter-spacing:-0.01em;white-space:nowrap;",
  "}",

  ".snk-success .snk-title{color:var(--toast-success-text);}",
  ".snk-error   .snk-title{color:var(--toast-error-text);}",

  ".snk-sub{font-size:11.5px;font-weight:400;line-height:1.3;white-space:nowrap;}",

  ".snk-success .snk-sub{color:var(--toast-success-sub);}",
  ".snk-error   .snk-sub{color:var(--toast-error-sub);}",

  /* CLOSE */
  ".snk-close{",
    "flex-shrink:0;width:20px;height:20px;border-radius:50%;",
    "border:none;background:transparent;cursor:pointer;",
    "display:none;align-items:center;justify-content:center;",
    "padding:0;margin-left:2px;",
    "transition:background 0.18s ease,color 0.18s ease;",
  "}",

  ".snk-success .snk-close{color:var(--toast-success-sub);}",
  ".snk-error   .snk-close{color:var(--toast-error-sub);}",

  ".snk-success .snk-close:hover{",
    "background:color-mix(in srgb, var(--toast-icon-success) 12%, transparent);",
    "color:var(--toast-icon-success);",
  "}",

  ".snk-error .snk-close:hover{",
    "background:color-mix(in srgb, var(--toast-icon-error) 12%, transparent);",
    "color:var(--toast-icon-error);",
  "}",

  ".snk-close svg{width:10px;height:10px;stroke:currentColor;stroke-width:2.2;stroke-linecap:round;fill:none;}",

  /* PROGRESS BAR */
 ".snk-bar-track{",
  "position:absolute;bottom:0;left:14px;right:14px;",
  "height:3px;border-radius:999px;",
  "overflow:hidden;",
  "background:rgba(0,0,0,0.06);",
"}",

".snk-success .snk-bar-track{",
  "background:rgba(52,168,100,0.12);",
"}",

".snk-error .snk-bar-track{",
  "background:rgba(220,60,60,0.12);",
"}",

".snk-bar{",
  "height:100%;",
  "width:100%;",
  "transform-origin:left;",
  "animation:snkDrain 2s linear forwards;",
"}",

".snk-success .snk-bar{",
  "background:linear-gradient(90deg,var(--toast-icon-success),rgba(74,222,128,0.6));",
"}",

".snk-error .snk-bar{",
  "background:linear-gradient(90deg,var(--toast-icon-error),rgba(248,113,113,0.6));",
"}",

".snk-toast:hover .snk-bar{animation-play-state:paused;}",

  /* EXIT */
  ".snk-toast.snk-out{animation:snkOut 0.28s cubic-bezier(0.55,0,1,0.45) forwards!important;}",

  /* ANIMATIONS */
  "@keyframes snkIn{to{opacity:1;transform:translateX(0) scale(1);}}",
  "@keyframes snkOut{to{opacity:0;transform:translateX(48px) scale(0.94);}}",

  "@keyframes snkPulseGreen{0%,100%{box-shadow:0 0 0 0 rgba(74,222,128,0.30);}50%{box-shadow:0 0 0 5px rgba(74,222,128,0.00);}}",
  "@keyframes snkPulseRed{0%,100%{box-shadow:0 0 0 0 rgba(248,113,113,0.25);}50%{box-shadow:0 0 0 5px rgba(248,113,113,0.00);}}",

  "@keyframes snkDrain{from{transform:scaleX(1);}to{transform:scaleX(0);}}"
].join("");
    document.head.appendChild(style);
  }

  /* ── Get or create the stack container ───────────────────────── */
  function getStack() {
    var stack = document.getElementById("snk-stack");
    if (!stack) {
      stack = document.createElement("div");
      stack.id = "snk-stack";
      stack.className = "snk-stack";
      document.body.appendChild(stack);
    }
    return stack;
  }

  /* ── SVG icons ────────────────────────────────────────────────── */
  var ICONS = {
    success: '<svg viewBox="0 0 14 14"><polyline points="2,7 5.5,10.5 12,3.5"/></svg>',
    error:   '<svg viewBox="0 0 14 14"><line x1="3" y1="3" x2="11" y2="11"/><line x1="11" y1="3" x2="3" y2="11"/></svg>'
  };
  var CLOSE_SVG = '<svg viewBox="0 0 10 10"><line x1="1" y1="1" x2="9" y2="9"/><line x1="9" y1="1" x2="1" y2="9"/></svg>';

  /* ── Core show function ───────────────────────────────────────── */
  function show(type, title, subtitle) {
    injectStyles();
    var stack = getStack();

    var toast = document.createElement("div");
    toast.className = "snk-toast snk-" + type;
    toast.setAttribute("role", "status");
    toast.setAttribute("aria-live", "polite");
    toast.innerHTML =
      '<div class="snk-icon">' + ICONS[type] + '</div>' +
      '<div class="snk-text">' +
        '<span class="snk-title">' + _esc(title) + '</span>' +
        (subtitle ? '<span class="snk-sub">' + _esc(subtitle) + '</span>' : '') +
      '</div>' +
      '<button class="snk-close" aria-label="Dismiss">' + CLOSE_SVG + '</button>' +
      '<div class="snk-bar-track"><div class="snk-bar"></div></div>';

    stack.appendChild(toast);

    /* dismiss helpers */
    var timer;
    function dismiss() {
      clearTimeout(timer);
      toast.classList.add("snk-out");
      toast.addEventListener("animationend", function () { toast.remove(); }, { once: true });
    }

    timer = setTimeout(dismiss, 2000);
    toast.querySelector(".snk-close").addEventListener("click", dismiss);
    toast.addEventListener("mouseenter", function () { clearTimeout(timer); });
    toast.addEventListener("mouseleave", function () { timer = setTimeout(dismiss, 2000); });
  }

  /* ── HTML escape helper ───────────────────────────────────────── */
  function _esc(str) {
    return String(str)
      .replace(/&/g, "&amp;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;")
      .replace(/"/g, "&quot;");
  }

  /* ── Public API ───────────────────────────────────────────────── */
  global.Toast = {
    success: function (title, subtitle) { show("success", title, subtitle || ""); },
    error:   function (title, subtitle) { show("error",   title, subtitle || ""); }
  };

}(window));