// import React from "react";

// export const Card: React.FC<{ className?: string; children: React.ReactNode }> = ({
//   className = "",
//   children,
// }) => (
//   <div className={`bg-white rounded-lg shadow ${className}`}>{children}</div>
// );

// export const CardContent: React.FC<{ className?: string; children: React.ReactNode }> = ({
//   className = "",
//   children,
// }) => (
//   <div className={`p-4 ${className}`}>{children}</div>
// );

// card.tsx
import React from "react";

interface CardProps {
  className?: string;
  onClick?: () => void;
  children: React.ReactNode;
}

export const Card: React.FC<CardProps> = ({
  className = "",
  onClick,
  children,
}) => (
  <div
    className={`bg-white rounded-lg shadow ${className}`}
    onClick={onClick}
  >
    {children}
  </div>
);

export const CardContent: React.FC<{ className?: string; children: React.ReactNode }> = ({
  className = "",
  children,
}) => (
  <div className={`p-4 ${className}`}>{children}</div>
);
