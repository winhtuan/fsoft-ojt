// core/dom.js
export const $ = (sel, root = document) => root.querySelector(sel);
export const $$ = (sel, root = document) => root.querySelectorAll(sel);

export function clearChildren(el) {
  while (el && el.firstChild) el.removeChild(el.firstChild);
}

export function show(el, show = true) {
  if (!el) return;
  el.hidden = !show;
}

export function toggle(el) {
  if (!el) return;
  el.hidden = !el.hidden;
}

export function setText(el, text) {
  if (el) el.textContent = text ?? "";
}
