import React from "react";
import { render } from "@testing-library/react";
import { ThemeProvider, createTheme, CssBaseline } from "@mui/material";

export function renderWithProviders(ui, options) {
  const Wrapper = ({ children }) => (
    <ThemeProvider theme={createTheme({ palette: { mode: "light" } })}>
      <CssBaseline />
      {children}
    </ThemeProvider>
  );
  return render(ui, { wrapper: Wrapper, ...options });
}
