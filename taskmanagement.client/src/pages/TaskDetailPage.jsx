import React, { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import {
    Stack,
    Typography,
    Button,
    FormControl,
    InputLabel,
    Select,
    MenuItem,
    TextField,
    Switch,
    FormControlLabel,
    Alert,
} from "@mui/material";
import useTask from "../hooks/useTask.js";
import { api } from "../services/api.js";
import { statusList, TaskStatusCode, nameFromCode } from "../constants/status.js";

export default function TaskDetailPage() {
    const { boardId = "", taskId = "" } = useParams();
    const navigate = useNavigate();

    const [userId, setUserId] = useState("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    const { data, loading, error, refetch } = useTask(boardId, taskId, userId);

    const [name, setName] = useState("");
    const [description, setDescription] = useState("");
    const [deadline, setDeadline] = useState("");
    const [status, setStatus] = useState(statusList[0]);
    const [favorite, setFavorite] = useState(false);

    const [saveBusy, setSaveBusy] = useState(false);
    const [favBusy, setFavBusy] = useState(false);
    const [formError, setFormError] = useState("");

    useEffect(() => {
        if (!data) return;
        setName(data.name || "");
        setDescription(data.description || "");
        setDeadline(data.deadline || "");
        setStatus(nameFromCode(data.status) || statusList[0]);
        setFavorite(!!data.isFavorite);
        setFormError("");
    }, [data?.id, data?.name, data?.description, data?.deadline, data?.status, data?.isFavorite]);

    async function saveTask() {
        if (!name.trim()) {
            setFormError("Name is required.");
            return;
        }
        setFormError("");
        setSaveBusy(true);
        try {
            await api(`/api/v1/boards/${boardId}/tasks/${taskId}`, {
                method: "PUT",
                body: JSON.stringify({
                    name: name.trim(),
                    description: description || null,
                    deadline: deadline || undefined,
                    status: TaskStatusCode[status],
                }),
            });
            await refetch();
        } catch (e) {
            setFormError(e.message || "Failed to save task");
        } finally {
            setSaveBusy(false);
        }
    }

    async function toggleFavorite(next) {
        if (!userId) {
            setFormError("UserId is required to set favorite.");
            return;
        }
        setFormError("");
        setFavBusy(true);
        try {
            if (next) {
                await api(`/api/v1/users/${userId}/favorites/${taskId}`, { method: "POST" });
            } else {
                await api(`/api/v1/users/${userId}/favorites/${taskId}`, { method: "DELETE" });
            }
            setFavorite(next);
            await refetch();
        } catch (e) {
            setFormError(e.message || "Failed to update favorite");
        } finally {
            setFavBusy(false);
        }
    }

    return (
        <Stack spacing={2}>
            <Button onClick={() => navigate(-1)} size="small" variant="text">
                ← Back
            </Button>

            {loading && <Typography>Loading...</Typography>}
            {error && <Typography color="error.main">{error}</Typography>}
            {formError && <Alert severity="error">{formError}</Alert>}

            {data && (
                <Stack spacing={2}>
                    <Typography variant="h5" fontWeight={600}>
                        Edit Task
                    </Typography>

                    <Stack direction={{ xs: "column", sm: "row" }} spacing={1} alignItems={{ xs: "stretch", sm: "center" }}>
                        <TextField
                            size="small"
                            label="UserId"
                            value={userId}
                            onChange={(e) => setUserId(e.target.value)}
                            sx={{ width: { xs: "100%", sm: 380 } }}
                        />
                        <FormControlLabel
                            control={
                                <Switch
                                    checked={favorite}
                                    onChange={(e) => toggleFavorite(e.target.checked)}
                                    disabled={favBusy}
                                />
                            }
                            label="Favorite (by user)"
                        />
                    </Stack>

                    <TextField
                        label="Name"
                        value={name}
                        onChange={(e) => setName(e.target.value)}
                        required
                        fullWidth
                    />

                    <TextField
                        label="Description"
                        value={description}
                        onChange={(e) => setDescription(e.target.value)}
                        multiline
                        minRows={3}
                        fullWidth
                    />

                    <Stack direction={{ xs: "column", sm: "row" }} spacing={2}>
                        <TextField
                            type="date"
                            label="Deadline"
                            value={deadline || ""}
                            onChange={(e) => setDeadline(e.target.value)}
                            InputLabelProps={{ shrink: true }}
                            sx={{ width: 240 }}
                        />

                        <FormControl size="small" sx={{ minWidth: 200 }}>
                            <InputLabel>Status</InputLabel>
                            <Select
                                label="Status"
                                value={status}
                                onChange={(e) => setStatus(e.target.value)}
                            >
                                {statusList.map((x) => (
                                    <MenuItem key={x} value={x}>
                                        {x}
                                    </MenuItem>
                                ))}
                            </Select>
                        </FormControl>
                    </Stack>

                    <Stack direction="row" spacing={1}>
                        <Button disabled={saveBusy} onClick={saveTask} variant="contained">
                            Save
                        </Button>
                        <Button onClick={() => refetch()} variant="outlined">
                            Reload
                        </Button>
                    </Stack>

                    <Stack spacing={0.5}>
                        <Typography variant="caption" color="text.secondary">
                            TaskId: {taskId}
                        </Typography>
                        <Typography variant="caption" color="text.secondary">
                            BoardId: {boardId}
                        </Typography>
                    </Stack>
                </Stack>
            )}
        </Stack>
    );
}
