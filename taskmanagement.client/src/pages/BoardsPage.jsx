import React from "react";
import { Link as RouterLink } from "react-router-dom";
import { Grid, Card, CardActionArea, CardContent, Typography } from "@mui/material";
import useBoards from "../hooks/useBoards.js";


export default function BoardsPage() {
    const { data, loading, error } = useBoards();
    return (
        <div>
            <Typography variant="h5" sx={{ mb: 2, fontWeight: 600 }}>Boards</Typography>
            {loading && <Typography color="text.secondary">Loading...</Typography>}
            {error && <Typography color="error.main">{error}</Typography>}
            <Grid container spacing={2}>
                {data?.result.map((b) => (
                    <Grid item xs={12} sm={6} md={4} key={b.id}>
                        <Card variant="outlined">
                            <CardActionArea component={RouterLink} to={`/boards/${b.id}`}>
                                <CardContent>
                                    <Typography variant="subtitle1" fontWeight={600}>{b.name}</Typography>
                                </CardContent>
                            </CardActionArea>
                        </Card>
                    </Grid>
                ))}
            </Grid>
        </div>
    );
}