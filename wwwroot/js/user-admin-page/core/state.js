// core/state.js
const state = {
  currentPage: 1,
  pageSize: 10,
  query: "",
  status: "",
};

export function getState() {
  return { ...state };
}

export function updateState(changes) {
  Object.assign(state, changes);
}
