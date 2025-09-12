import React from "react";
import { NavLink } from "react-router-dom";
import { Card, CardHeader, CardContent, Button, Chip, Stack, Box, Typography } from "@mui/material";
import TaskQuickActions from "./TaskQuickActions.jsx";


export default function Column({ title, hook, boardId }) {
    const { data, loading, error, refetch } = hook;
    return (
        <Card>
            <CardHeader titleTypographyProps={{ variant: "subtitle1" }} title={title} action={<Button size="small" variant="outlined" onClick={refetch}>Refresh</Button>} />
            <CardContent>
                {loading && <Typography variant="body2" color="text.secondary">Loading...</Typography>}
                {error && <Typography variant="body2" color="error.main">{error}</Typography>}
                <Stack spacing={1.5}>
                    {data?.map((t) => (
                        <Box key={t.id} sx={{ border: 1, borderColor: "divider", borderRadius: 2, p: 1.5 }}>
                            <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between", gap: 1 }}>
                                <Typography component={NavLink} to={`/boards/${boardId}/tasks/${t.id}`} sx={{ textDecoration: "none", color: "text.primary", fontWeight: 600 }}>
                                    {t.name}
                                </Typography>
                                <Chip label={t.isFavorite ? "★ Favorite" : "☆"} color={t.isFavorite ? "warning" : "default"} size="small" variant={t.isFavorite ? "filled" : "outlined"} />
                            </Box>
                            <TaskQuickActions task={t} boardId={boardId} onChanged={refetch} />
                        </Box>
                    ))}
                </Stack>
            </CardContent>
        </Card>
    );
}