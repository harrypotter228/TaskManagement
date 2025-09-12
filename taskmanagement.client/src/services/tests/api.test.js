import {
  api,
  fetchBoards,
  fetchTask,
  createTask,
  updateTaskStatus,
  fetchTasksSorted,
  fetchTasksByStatuses,
} from "../api";

describe("services/api", () => {
  beforeEach(() => {
    global.fetch = jest.fn();
  });

  afterEach(() => {
    jest.resetAllMocks();
  });

  test("api returns JSON on ok", async () => {
    global.fetch.mockResolvedValueOnce({
      ok: true,
      status: 200,
      json: async () => ({ ok: 1 }),
    });
    await expect(api("/x")).resolves.toEqual({ ok: 1 });
  });

  test("api throws on !ok using message", async () => {
    global.fetch.mockResolvedValueOnce({
      ok: false,
      status: 400,
      json: async () => ({ message: "Bad" }),
    });
    await expect(api("/x")).rejects.toThrow("Bad");
  });

  test("api returns undefined on 204", async () => {
    global.fetch.mockResolvedValueOnce({
      ok: true,
      status: 204,
      json: async () => ({}),
    });
    await expect(api("/x")).resolves.toBeUndefined();
  });

  test("fetchBoards url", async () => {
    global.fetch.mockResolvedValueOnce({ ok: true, status: 200, json: async () => [] });
    await fetchBoards();
    expect(global.fetch).toHaveBeenCalledWith(
      "/api/v1/boards?includeStats=false",
      expect.any(Object)
    );
  });

  test("fetchTask with userId", async () => {
    global.fetch.mockResolvedValueOnce({ ok: true, status: 200, json: async () => ({}) });
    await fetchTask("b1", "t1", "u1");
    expect(global.fetch).toHaveBeenCalledWith(
      "/api/v1/boards/b1/tasks/t1?userId=u1",
      expect.any(Object)
    );
  });

  test("createTask POST", async () => {
    global.fetch.mockResolvedValueOnce({
      ok: true,
      status: 201,
      json: async () => ({ id: "t1" }),
    });
    await createTask("b1", { name: "N" });
    expect(global.fetch).toHaveBeenCalledWith(
      "/api/v1/boards/b1/tasks",
      expect.objectContaining({
        method: "POST",
        body: JSON.stringify({ name: "N" }),
      })
    );
  });

  test("fetchTasksSorted query", async () => {
    global.fetch.mockResolvedValueOnce({ ok: true, status: 200, json: async () => [] });
    await fetchTasksSorted("b1", "u1", "ToDo");
    expect(global.fetch).toHaveBeenCalledWith(
      "/api/v1/boards/b1/tasks/sorted?userId=u1&status=ToDo",
      expect.any(Object)
    );
  });

  test("fetchTasksByStatuses multiple params", async () => {
    global.fetch.mockResolvedValueOnce({ ok: true, status: 200, json: async () => [] });
    await fetchTasksByStatuses("b1", "u1", ["ToDo", "Done"]);
    const url = global.fetch.mock.calls[0][0];
    expect(url).toMatch(/^\/api\/v1\/boards\/b1\/tasks\/by-status\?/);
    expect(url).toContain("userId=u1");
    expect((url.match(/statuses=/g) || []).length).toBe(2);
  });
});
