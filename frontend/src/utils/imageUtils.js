export const convertBytesToBase64Image = (imageBytes) => {
  if (!imageBytes) return null;
  return `data:image/jpeg;base64,${imageBytes}`;
};
