import { useEffect, useState } from "react";
import { fetchTasksSorted, api } from "../services/api.js";


export default function useTasksSorted(boardId, userId, status) {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    useEffect(() => {
        let alive = true;
        setLoading(true);
        fetchTasksSorted(boardId, userId, status)
            .then((d) => { if (alive) { setData(d); setError(null); } })
            .catch((e) => alive && setError(e.message))
            .finally(() => alive && setLoading(false));
        return () => { alive = false; };
    }, [boardId, userId, status]);
    return { data, loading, error, refetch: async () => setData(await api(`/api/v1/boards/${boardId}/tasks/sorted?userId=${userId}&status=${status}`)) };
}