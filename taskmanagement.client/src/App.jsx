import React from "react";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import AppThemeProvider from "./theme/ThemeProvider.jsx";
import Layout from "./components/Layout.jsx";
import BoardsPage from "./pages/BoardsPage.jsx";
import BoardPage from "./pages/BoardPage.jsx";
import TaskDetailPage from "./pages/TaskDetailPage.jsx";
import NotFound from "./pages/NotFound.jsx";


export default function App() {
    return (
        <AppThemeProvider>
            <BrowserRouter>
                <Layout>
                    <Routes>
                        <Route path="/" element={<BoardsPage />} />
                        <Route path="/boards" element={<BoardsPage />} />
                        <Route path="/boards/:boardId" element={<BoardPage />} />
                        <Route path="/boards/:boardId/tasks/:taskId" element={<TaskDetailPage />} />
                        <Route path="*" element={<NotFound />} />
                    </Routes>
                </Layout>
            </BrowserRouter>
        </AppThemeProvider>
    );
}