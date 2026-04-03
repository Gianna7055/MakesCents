export const formatDate = (date: any): string => {
  //console.log("Date:", date);
  const year = date.year;
  const month = date.month;
  const day = date.day;

  return `${month}/${day}`;
};
