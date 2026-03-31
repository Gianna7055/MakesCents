
export const formatDate = (date: any): string => {
    const dateString = date as string;
    const [year, month, day] = dateString.split('-').map(Number);
    return `${month}/${day}`;
  };
