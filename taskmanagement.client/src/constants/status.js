export const statusList = ["ToDo", "InProgress", "Done"];
export const TaskStatusCode = Object.freeze({ ToDo: 0, InProgress: 1, Done: 2 });
export function nameFromCode(code) {
  return Object.keys(TaskStatusCode).find(k => TaskStatusCode[k] === code);
}
