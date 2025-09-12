import React, { createContext, useMemo, useState } from "react";
import { ThemeProvider, createTheme, CssBaseline } from "@mui/material";


export const ColorModeContext = createContext({ toggleColorMode: () => { } });


export default function AppThemeProvider({ children }) {
    const [mode, setMode] = useState(() => localStorage.getItem("mui-mode") || "light");
    const colorMode = useMemo(
        () => ({
            toggleColorMode: () => {
                setMode((prev) => {
                    const next = prev === "light" ? "dark" : "light";
                    localStorage.setItem("mui-mode", next);
                    return next;
                });
            },
        }),
        []
    );


    const theme = useMemo(
        () =>
            createTheme({
                palette: { mode },
                shape: { borderRadius: 12 },
                components: {
                    MuiCard: { styleOverrides: { root: { borderWidth: 1, borderStyle: "solid", borderColor: "divider" } } },
                },
            }),
        [mode]
    );


    return (
        <ColorModeContext.Provider value={colorMode}>
            <ThemeProvider theme={theme}>
                <CssBaseline />
                {children}
            </ThemeProvider>
        </ColorModeContext.Provider>
    );
}