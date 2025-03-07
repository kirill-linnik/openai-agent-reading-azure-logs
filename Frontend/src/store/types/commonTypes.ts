export const COMMON_REMOVE_ERROR = "@COMMON/REMOVE_ERROR";
export const COMMON_REMOVE_INFO = "@COMMON/REMOVE_INFO";

export type CommonState = {
  error?: string;
  info?: string;
  needsRefresh?: boolean;
};

type CommonRemoveError = {
  type: typeof COMMON_REMOVE_ERROR;
};

type CommonRemoveInfo = {
  type: typeof COMMON_REMOVE_INFO;
};

export type CommonAction = CommonRemoveError | CommonRemoveInfo;
