import { AxiosResponse } from 'axios';
import { useQuery, UseQueryResult } from 'react-query';
import axiosInstance from '../common/axios';
import { CONFIG } from '../common/Configuration';
import { GET_ALL_PROJECT_KEY } from '../common/QueryKeys';
import { IProject } from '../models/IProject';

export default function useProjects(): UseQueryResult<IProject[], Error> {
  return useQuery<IProject[], Error>(GET_ALL_PROJECT_KEY, () =>
    axiosInstance
      .get(`${CONFIG.API_BASE_URL}/projects`)
      .then((res: AxiosResponse<IProject[]>) => res.data)
  );
}
