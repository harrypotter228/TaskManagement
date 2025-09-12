import React from "react";
import { Link as RouterLink } from "react-router-dom";
import { Stack, Typography, Button } from "@mui/material";


export default function NotFound() {
    return (
        <Stack spacing={1} sx={{ py: 4 }}>
            <Typography variant="h4" fontWeight={700}>404</Typography>
            <Typography color="text.secondary">Not found</Typography>
            <Button component={RouterLink} to="/boards" variant="text">Go to Boards</Button>
        </Stack>
    );
}