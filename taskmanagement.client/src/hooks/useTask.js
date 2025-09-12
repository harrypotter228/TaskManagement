import { useEffect, useState } from "react";
import { fetchTask, api } from "../services/api.js";


export default function useTask(boardId, taskId, userId) {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    useEffect(() => {
        let alive = true;
        setLoading(true);
        fetchTask(boardId, taskId, userId)
            .then((d) => { if (alive) { setData(d); setError(null); } })
            .catch((e) => alive && setError(e.message))
            .finally(() => alive && setLoading(false));
        return () => { alive = false; };
    }, [boardId, taskId, userId]);
    return { data, loading, error, refetch: async () => setData(await api(`/api/v1/boards/${boardId}/tasks/${taskId}${userId ? `?userId=${userId}` : ""}`)) };
}