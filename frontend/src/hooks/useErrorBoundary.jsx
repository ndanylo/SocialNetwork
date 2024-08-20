import { useState } from "react";

const useErrorBoundary = () => {
  const [errorState, setErrorState] = useState({
    hasError: false,
    errorMessage: null,
  });

  const handleError = (error) => {
    setErrorState({
      hasError: true,
      errorMessage: error.message,
    });
  };

  const removeError = () => {
    setErrorState({
      hasError: false,
      errorMessage: null,
    });
  };

  return {
    hasError: errorState.hasError,
    errorMessage: errorState.errorMessage,
    removeError,
    handleError,
  };
};

export default useErrorBoundary;
