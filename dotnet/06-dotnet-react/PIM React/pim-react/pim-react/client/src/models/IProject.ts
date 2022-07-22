import { ProjectStatus } from './ProjectStatusEnum';

export interface IProject {
  id:        number;
  startDate: string;
  endDate:   string;
  number:    number;
  name:      string;
  customer:  string;
  group:     string;
  members:   string;
  status:    ProjectStatus;
}
