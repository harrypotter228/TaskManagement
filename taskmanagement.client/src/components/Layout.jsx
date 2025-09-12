import React, { useContext } from "react";
import { Link as RouterLink, NavLink } from "react-router-dom";
import { AppBar, Toolbar, Typography, Container, Button, IconButton, Box } from "@mui/material";
import Brightness4Icon from "@mui/icons-material/Brightness4";
import Brightness7Icon from "@mui/icons-material/Brightness7";
import { ColorModeContext } from "../theme/ThemeProvider.jsx";


export default function Layout({ children }) {
    const { toggleColorMode } = useContext(ColorModeContext);
    const mode = (typeof window !== "undefined" && localStorage.getItem("mui-mode")) || "light";


    return (
        <Box sx={{ minHeight: "100vh", bgcolor: "background.default", color: "text.primary" }}>
            <AppBar position="sticky" color="default" elevation={0} sx={{ borderBottom: 1, borderColor: "divider" }}>
                <Toolbar>
                    <Typography variant="h6" component={RouterLink} to="/boards" sx={{ textDecoration: "none", color: "inherit", fontWeight: 700, mr: 2 }}>
                        TaskManagementApp
                    </Typography>
                    <Button component={RouterLink} to="/boards" color="inherit" sx={{ mr: 1 }}>
                        Boards
                    </Button>
                    <Box sx={{ flexGrow: 1 }} />
                    <IconButton onClick={toggleColorMode} color="inherit" size="small" aria-label="toggle theme">
                        {mode === "dark" ? <Brightness7Icon /> : <Brightness4Icon />}
                    </IconButton>
                </Toolbar>
            </AppBar>
            <Container maxWidth="lg" sx={{ py: 3 }}>{children}</Container>
        </Box>
    );
}