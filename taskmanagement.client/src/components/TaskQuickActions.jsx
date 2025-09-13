import React, { useEffect, useState } from "react";
import { FormControl, InputLabel, Select, MenuItem, Typography } from "@mui/material";
import { updateTaskStatus } from "../services/api.js";
import { statusList, nameFromCode } from "../constants/status.js";

export default function TaskQuickActions({ task, boardId, onChanged }) {
  const [s, setS] = useState(task?.status ?? "ToDo");
  const [busy, setBusy] = useState(false);

  useEffect(() => {
    setS(task?.status ? nameFromCode(task?.status) : "ToDo");
  }, [task?.id, task?.status]);

  async function onChangeStatus(next) {
    setBusy(true);
    try {
      await updateTaskStatus(boardId, task.id, next);
      setS(next);
      onChanged?.();
    } finally {
      setBusy(false);
    }
  }

  return (
    <div style={{ marginTop: 8, display: "flex", alignItems: "center", gap: 8 }}>
      <FormControl size="small" sx={{ minWidth: 160 }} disabled={busy}>
        <InputLabel id={`status-${task.id}`}>Status</InputLabel>
        <Select
          labelId={`status-${task.id}`}
          label="Status"
          value={s}
          onChange={(e) => onChangeStatus(e.target.value)}
        >
          {statusList.map((x) => (
            <MenuItem key={x} value={x}>
              {x}
            </MenuItem>
          ))}
        </Select>
      </FormControl>
      {task.deadline && (
        <Typography variant="caption" sx={{ marginLeft: "auto", color: "text.secondary" }}>
          Due {task.deadline}
        </Typography>
      )}
    </div>
  );
}
