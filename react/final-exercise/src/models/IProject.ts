import { ProjectStatus } from './ProjectStatusEnum';

export interface IProject {
  id:        string;
  startDate: Date;
  endDate:   Date;
  number:    number;
  name:      string;
  customer:  string;
  group:     string;
  members:   string;
  status:    ProjectStatus;
}
