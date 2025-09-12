import React, { useState } from "react";
import { Dialog, DialogTitle, DialogContent, DialogActions, Button, TextField, Stack } from "@mui/material";
import { createTask } from "../services/api.js";


export default function CreateTaskModal({ boardId, onClose }) {
    const [name, setName] = useState("");
    const [description, setDescription] = useState("");
    const [deadline, setDeadline] = useState("");
    const [busy, setBusy] = useState(false);


    async function submit(e) {
        e.preventDefault();
        setBusy(true);
        try { await createTask(boardId, { name, description, deadline: deadline || undefined }); onClose(); } finally { setBusy(false); }
    }


    return (
        <Dialog open onClose={onClose} fullWidth maxWidth="sm">
            <DialogTitle>New Task</DialogTitle>
            <form onSubmit={submit}>
                <DialogContent>
                    <Stack spacing={2}>
                        <TextField label="Name" value={name} onChange={(e) => setName(e.target.value)} required fullWidth />
                        <TextField label="Description" value={description} onChange={(e) => setDescription(e.target.value)} multiline minRows={3} fullWidth />
                        <TextField label="Deadline" type="date" value={deadline} onChange={(e) => setDeadline(e.target.value)} InputLabelProps={{ shrink: true }} fullWidth />
                    </Stack>
                </DialogContent>
                <DialogActions>
                    <Button onClick={onClose} variant="outlined">Cancel</Button>
                    <Button type="submit" disabled={busy} variant="contained">Create</Button>
                </DialogActions>
            </form>
        </Dialog>
    );
}