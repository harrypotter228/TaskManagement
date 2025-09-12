import React, { useState } from "react";
import { Link as RouterLink, useParams } from "react-router-dom";
import { Grid, Typography, Stack, Button, TextField } from "@mui/material";
import { statusList } from "../constants/status.js";
import useTasksSorted from "../hooks/useTasksSorted.js";
import Column from "../components/Column.jsx";
import CreateTaskModal from "../components/CreateTaskModal.jsx";


export default function BoardPage() {
    const params = useParams();
    const boardId = params.boardId || "";
    const [userId, setUserId] = useState("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    const [creating, setCreating] = useState(false);
    const cols = statusList.map((s) => ({ status: s, hook: useTasksSorted(boardId, userId, s) }));

    return (
        <Stack spacing={2}>
            <Stack direction="row" alignItems="center" justifyContent="space-between">
                <Typography variant="h5" fontWeight={600}>Board #{boardId.slice(0, 8)}</Typography>
                <Stack direction="row" spacing={1} alignItems="center">
                    <div className="flex items-center gap-2">
                        <TextField size="small" value={userId} onChange={(e) => setUserId(e.target.value)} label="UserId" sx={{ width: 360 }} />
                        <Button variant="contained" onClick={() => setCreating(true)}>New Task</Button>
                    </div>
                </Stack>
            </Stack>
            <Grid container spacing={2}>
                {cols.map(({ status, hook }) => (
                    <Grid item xs={12} md={4} key={status}>
                        <Column title={status} hook={hook} boardId={boardId} />
                    </Grid>
                ))}
            </Grid>
            {creating && <CreateTaskModal boardId={boardId} onClose={() => setCreating(false)} />}
        </Stack>
    );
}