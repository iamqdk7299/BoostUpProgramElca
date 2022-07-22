import axios, { AxiosInstance, AxiosResponse } from 'axios';

let axiosInstance: AxiosInstance = axios.create();

axiosInstance.interceptors.response.use(async (response: AxiosResponse<any>) => {
  await new Promise(resolve => setTimeout(resolve, 2000));
  return response;
});

export default axiosInstance;