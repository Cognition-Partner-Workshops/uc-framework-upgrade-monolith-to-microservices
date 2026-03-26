export interface LossReport {
  id: number;
  policyNumber: string;
  description: string;
  amount: number;
  createdDate: string;
}

export interface CreateLossReport {
  policyNumber: string;
  description: string;
  amount: number;
}

export interface UpdateLossReport {
  policyNumber: string;
  description: string;
  amount: number;
}
