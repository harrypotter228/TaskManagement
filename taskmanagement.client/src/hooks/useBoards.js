import { useEffect, useState } from "react";
import { fetchBoards, api } from "../services/api.js";


export default function useBoards() {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    useEffect(() => {
        let alive = true;
        setLoading(true);
        fetchBoards()
            .then((d) => { if (alive) { setData(d); setError(null); } })
            .catch((e) => alive && setError(e.message))
            .finally(() => alive && setLoading(false));
        return () => { alive = false; };
    }, []);
    return { data, loading, error, refetch: async () => setData(await api(`/api/v1/boards?includeStats=false`)) };
}