import { LinearProgress } from '@material-ui/core';
import { DataGrid, GridCellParams, GridColumns } from '@material-ui/data-grid';
import React from 'react';
import useProjects from '../../hooks/useProjects';

const renderDate = (params: GridCellParams) => <>{params.value ? (new Date(params.value as string)).toLocaleDateString('de-CH') : ''}</>

const columns: GridColumns = [
  {
    field: 'number',
    headerName: 'Number',
    width: 150
  },
  {
    field: 'name',
    headerName: 'Name',
    width: 400
  },
  {
    field: 'customer',
    headerName: 'Customer',
    width: 400
  },
  {
    field: 'startDate',
    headerName: 'Start Date',
    width: 150,
    renderCell: renderDate
  },
  {
    field: 'endDate',
    headerName: 'End Date',
    width: 150,
    renderCell: renderDate
  },
  {
    field: 'group',
    headerName: 'Group',
    width: 150
  },
  {
    field: 'members',
    headerName: 'Members',
    width: 300
  },
  {
    field: 'status',
    headerName: 'Status',
    width: 150
  },
];

const ProjectList = (): JSX.Element => {
  const { data, isLoading } = useProjects();
  return isLoading ? (
    <LinearProgress />
  ) : (
    <div style={{ height: '70vh', width: '100%' }}>
    <div style={{ display: 'flex', height: '100%' }}>
      <div style={{ flexGrow: 1 }}>
        <DataGrid rows={data || []} columns={columns} />
      </div>
    </div>
    </div>
  );
};

export default ProjectList;
