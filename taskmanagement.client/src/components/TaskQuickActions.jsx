// src/components/tests/TaskQuickActions.test.jsx
import React from "react";
import { screen, fireEvent, within } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { renderWithProviders } from "@/tests/test-utils";
import TaskQuickActions from "../TaskQuickActions";
import { updateTaskStatus } from "../../services/api.js";

jest.mock("../../services/api.js", () => ({
  updateTaskStatus: jest.fn().mockResolvedValue({}),
}));

const boardId = "b-1";

async function openMenu() {
  const trigger = screen.getByLabelText(/status/i);
  fireEvent.mouseDown(trigger);
  return await screen.findByRole("listbox");
}

describe("TaskQuickActions", () => {
  test("shows initial status from numeric code", () => {
    renderWithProviders(<TaskQuickActions task={{ id: "t-1", status: 2 }} boardId={boardId} onChanged={() => {}} />);
    expect(screen.getByRole("button", { name: /done/i })).toBeInTheDocument();
  });

  test("change status calls api and onChanged", async () => {
    const user = userEvent.setup();
    const onChanged = jest.fn();
    renderWithProviders(<TaskQuickActions task={{ id: "t-2", status: 0 }} boardId={boardId} onChanged={onChanged} />);

    const listbox = await openMenu();
    const option = within(listbox).getByText("InProgress");
    await user.click(option);

    expect(updateTaskStatus).toHaveBeenCalledWith(boardId, "t-2", "InProgress");
    expect(onChanged).toHaveBeenCalledTimes(1);
  });

  test("syncs when task prop changes", async () => {
    const { rerender } = renderWithProviders(
      <TaskQuickActions task={{ id: "t-3", status: 0 }} boardId={boardId} onChanged={() => {}} />
    );
    expect(screen.getByRole("button", { name: /todo/i })).toBeInTheDocument();

    rerender(<TaskQuickActions task={{ id: "t-3", status: 1 }} boardId={boardId} onChanged={() => {}} />);
    expect(screen.getByRole("button", { name: /inprogress/i })).toBeInTheDocument();
  });
});
