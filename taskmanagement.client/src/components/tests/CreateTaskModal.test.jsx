import React from "react";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import CreateTaskModal from "../CreateTaskModal";
import * as api from "../../services/api.js";

jest.mock("../../services/api.js", () => ({
  createTask: jest.fn().mockResolvedValue({}),
}));

describe("CreateTaskModal", () => {
  const boardId = "b-1";
  const onClose = jest.fn();

  beforeEach(() => {
    jest.clearAllMocks();
    onClose.mockClear();
  });

  test("renders all input fields", () => {
    render(<CreateTaskModal boardId={boardId} onClose={onClose} />);
    expect(screen.getByLabelText(/name/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/description/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/deadline/i)).toBeInTheDocument();
  });

  test("calls createTask and onClose on submit", async () => {
    render(<CreateTaskModal boardId={boardId} onClose={onClose} />);
    fireEvent.change(screen.getByLabelText(/name/i), { target: { value: "Task 1" } });
    fireEvent.change(screen.getByLabelText(/description/i), { target: { value: "Desc" } });
    fireEvent.change(screen.getByLabelText(/deadline/i), { target: { value: "2025-09-30" } });

    fireEvent.click(screen.getByRole("button", { name: /create/i }));

    await waitFor(() => {
      expect(api.createTask).toHaveBeenCalledWith(boardId, {
        name: "Task 1",
        description: "Desc",
        deadline: "2025-09-30",
      });
      expect(onClose).toHaveBeenCalled();
    });
  });

  test("calls onClose when cancel is clicked", () => {
    render(<CreateTaskModal boardId={boardId} onClose={onClose} />);
    fireEvent.click(screen.getByRole("button", { name: /cancel/i }));
    expect(onClose).toHaveBeenCalled();
  });

  test("submit disables button while busy", async () => {
    render(<CreateTaskModal boardId={boardId} onClose={onClose} />);
    fireEvent.change(screen.getByLabelText(/name/i), { target: { value: "Task 2" } });
    fireEvent.click(screen.getByRole("button", { name: /create/i }));
    expect(screen.getByRole("button", { name: /create/i })).toBeDisabled();
    await waitFor(() => expect(api.createTask).toHaveBeenCalled());
  });
});