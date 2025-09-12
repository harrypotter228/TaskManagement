import React from "react";
import { DataGrid } from "@mui/x-data-grid";
import { Chip } from "@mui/material";


export default function TaskTable({ tasks, onRowClick }) {
    const columns = [
        { field: "name", headerName: "Name", flex: 1, minWidth: 180 },
        { field: "description", headerName: "Description", flex: 1, minWidth: 220 },
        { field: "deadline", headerName: "Deadline", width: 120 },
        {
            field: "status",
            headerName: "Status",
            width: 130,
            renderCell: (params) => (
                <Chip
                    label={params.value}
                    color={params.value === "Done" ? "success" : params.value === "InProgress" ? "warning" : "default"}
                    size="small"
                    variant={params.value === "ToDo" ? "outlined" : "filled"}
                />
            ),
        },
        {
            field: "isFavorite",
            headerName: "Favorite",
            width: 110,
            renderCell: (p) => (p.value ? <span style={{ color: "gold" }}>★</span> : "☆"),
            sortable: true,
        },
    ];


    return (
        <div style={{ height: 560, width: "100%" }}>
            <DataGrid
                rows={tasks || []}
                columns={columns}
                pageSizeOptions={[5, 10, 20]}
                initialState={{ pagination: { paginationModel: { pageSize: 10, page: 0 } } }}
                getRowId={(row) => row.id}
                onRowClick={(params) => onRowClick && onRowClick(params.row)}
                disableRowSelectionOnClick
            />
        </div>
    );
}